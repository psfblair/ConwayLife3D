namespace ConwayLife3D.Controllers

open UnityEngine
open ConwayLife3D.Life.Core
open System.Collections

type GameController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable pauseBetweenGenerations: float32 = 1.0f

    [<SerializeField>] [<DefaultValue>] val mutable token: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable reaper: GameObject

    let firstGeneration: Generation = 
        Set.ofList [
            ( 0,  0, -1);
            ( 0, -1,  0);
            (-1,  0,  0);
            ( 1,  0,  0);
            ( 0,  1,  0);
            (-1,  0,  1);
            ( 1,  0,  1) 
        ]

    let nextGenerationPairedWithPrevious (previousGeneration, generation)  =
        let nextGen = nextGeneration generation
        (generation, nextGen)
        
    let generationSequence =
        Seq.unfold (fun generationPair -> Some(generationPair, nextGenerationPairedWithPrevious generationPair)) (firstGeneration, firstGeneration)

    member this.Start () =
        this.Initialize()
        this.StartCoroutine (this.RunGame())        

    member private this.Initialize() =
        Set.iter this.BirthAt firstGeneration

    member private this.RunGame (): IEnumerator =
        let initialWait = seq { yield WaitForSeconds(pauseBetweenGenerations) }
        let waitSequence = generationSequence 
                            |> Seq.map (fun generationPair -> this.UpdateScene((fst generationPair), (snd generationPair))) 
                            |> Seq.append initialWait
        waitSequence.GetEnumerator() :> IEnumerator

    member private this.UpdateScene (previousGeneration: Generation, nextGeneration: Generation) =
        let cellsToDestroy = previousGeneration - nextGeneration
        Set.iter this.DeathAt cellsToDestroy

        let cellsToCreate =  nextGeneration - previousGeneration
        Set.iter this.BirthAt cellsToCreate
        WaitForSeconds(pauseBetweenGenerations)

    member private this.BirthAt (position: Cell) : Unit = 
        match position with
            | (x, y, z)     -> Object.Instantiate(this.token, new Vector3(float32 x, float32 y, float32 z), Quaternion.identity) |> ignore

    member private this.DeathAt (position: Cell) : Unit =
        match position with
            | (x, y, z)     -> Object.Instantiate(this.reaper, new Vector3(float32 x, float32 y, float32 z), Quaternion.identity) |> ignore
