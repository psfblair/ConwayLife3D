namespace ConwayLife3D.Controllers

open UnityEngine
open ConwayLife3D
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

    let nextGenerationPairedWithPrevious (previousGeneration, generation): Generation * Generation  =
        let nextGen = nextGeneration generation
        (generation, nextGen)
        
    let generationSequence: seq<Generation * Generation> =
        Seq.unfold (fun generationPair -> Some(generationPair, nextGenerationPairedWithPrevious generationPair)) (Set.empty, firstGeneration)

    let unityCoordinatesFrom (cell: Cell): Vector3 = 
        match cell with
            | (x, y, z)     -> new Vector3(float32 x, float32 y, float32 z)

    member this.Start () =
        this.StartCoroutine (this.RunGame())        

    member private this.RunGame (): IEnumerator =
        let initialWait = seq { yield WaitForSeconds(pauseBetweenGenerations) }
        let waitSequence = generationSequence 
                            |> Seq.map (fun generationPair -> this.UpdateSceneAndWait(generationPair)) 
                            |> Seq.append initialWait

        waitSequence.GetEnumerator() :> IEnumerator

    member private this.UpdateSceneAndWait (previousGeneration: Generation, nextGeneration: Generation) =
        let cellsToDestroy = previousGeneration - nextGeneration
        Set.iter this.DeathAt cellsToDestroy

        let cellsToCreate =  nextGeneration - previousGeneration
        Set.iter this.BirthAt cellsToCreate

        WaitForSeconds(pauseBetweenGenerations)

    member private this.BirthAt (cell: Cell) : Unit = 
        Object.Instantiate(this.token, (unityCoordinatesFrom cell), Quaternion.identity) |> ignore

    member private this.DeathAt (cell: Cell) : Unit =
        Object.Instantiate(this.reaper, (unityCoordinatesFrom cell), Quaternion.identity) |> ignore
