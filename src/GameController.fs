namespace ConwayLife3D.Controllers

open UnityEngine
open UnityEngine.UI
open ConwayLife3D
open ConwayLife3D.Patterns
open ConwayLife3D.Life.Core
open System.Collections

type GameController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable pauseBetweenGenerations: float32 = 1.0f

    [<SerializeField>] [<DefaultValue>] val mutable token: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable reaper: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable startPanel: GameObject

    let nextGenerationPairedWithPrevious (previousGeneration, generation): Generation * Generation  =
        let nextGen = nextGeneration generation
        (generation, nextGen)
        
    let generationSequence (firstGeneration: Generation): seq<Generation * Generation> =
        Seq.unfold (fun generationPair -> Some(generationPair, nextGenerationPairedWithPrevious generationPair)) (Set.empty, firstGeneration)

    let unityCoordinatesFrom (cell: Cell): Vector3 = 
        match cell with
            | (x, y, z)     -> new Vector3(float32 x, float32 y, float32 z)

    member this.Start () =
        this.startPanel.gameObject.SetActive(true)

    member this.StartGame (patternName: string) =
        this.startPanel.gameObject.SetActive(false)
        let patternsType = typeof<PatternModuleTypeAccessor>.DeclaringType
        let selectedPattern = patternsType.GetProperty(patternName).GetValue(null, Array.empty) :?> Generation
        this.StartCoroutine (this.RunGame selectedPattern) |> ignore
        ()

    member private this.RunGame (firstGeneration: Generation): IEnumerator =
        let initialWait = seq { yield WaitForSeconds(pauseBetweenGenerations) }
        let waitSequence = generationSequence firstGeneration
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
