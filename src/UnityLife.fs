namespace ConwayLife3D.Unity

open UnityEngine
open System.Collections
open ConwayLife3D.Life.Core

type GenerationTransition = Generation * Generation
type GameState = 
    | Running of float32
    | Restarting
    | Paused of GenerationTransition * float32

type UnityLife(token: GameObject, reaper: GameObject, pauseBetweenGenerations: float32, pauseWait: float32) =

    let mutable gameState = Running Time.time

    let nextGenerationPairedWithPrevious (previousGeneration, generation): GenerationTransition =
        let nextGen = nextGeneration generation
        (generation, nextGen)

    let generationSequence (firstGenerationPair: GenerationTransition): seq<GenerationTransition> =
        Seq.unfold (fun generationPair -> Some(generationPair, nextGenerationPairedWithPrevious generationPair)) firstGenerationPair

    let unityCoordinatesFrom (cell: Cell): Vector3 =
        match cell with
            | (x, y, z)     -> new Vector3(float32 x, float32 y, float32 z)

    member this.RunGame (generationTransition: GenerationTransition): IEnumerator =
        let initialWait = seq { yield WaitForSeconds(pauseBetweenGenerations) }
        let waitSequence = generationSequence generationTransition
                            |> Seq.takeWhile (fun generationPair -> this.ShouldContinue(generationPair))
                            |> Seq.map (fun generationPair -> this.UpdateSceneAndWait(generationPair)) 
                            |> Seq.append initialWait

        waitSequence.GetEnumerator() :> IEnumerator

    member this.GameState = gameState

    member this.PauseToggled timeOfLastToggle =
        Input.GetKey(KeyCode.P) && this.PauseWaitElapsed(timeOfLastToggle)


    member private this.UpdateSceneAndWait (previousGeneration: Generation, nextGeneration: Generation) =
        let cellsToDestroy = previousGeneration - nextGeneration
        Set.iter this.DeathAt cellsToDestroy

        let cellsToCreate =  nextGeneration - previousGeneration
        Set.iter this.BirthAt cellsToCreate

        WaitForSeconds(pauseBetweenGenerations)

    member private this.BirthAt (cell: Cell) : Unit =
        Object.Instantiate(token, (unityCoordinatesFrom cell), Quaternion.identity) |> ignore

    member private this.DeathAt (cell: Cell) : Unit =
        Object.Instantiate(reaper, (unityCoordinatesFrom cell), Quaternion.identity) |> ignore

    member private this.ShouldContinue (generationPair: GenerationTransition) =
        match gameState with 
            | Running startTime when this.PauseToggled startTime ->
                gameState <- Paused(generationPair, Time.time)
                false
            | Running startTime when Input.GetKey(KeyCode.R) ->
                gameState <- Restarting
                false
            | _ -> true

    member private this.PauseWaitElapsed timeOfLastToggle = timeOfLastToggle + pauseWait < Time.time
