namespace ConwayLife3D.Unity

open ConwayLife3D.Life.Core
open ConwayLife3D.Life.Game
open ConwayLife3D.Unity.Utils

open UnityEngine
open System.Collections

type GameState = 
    | Running of float32
    | Restarting
    | Paused of GenerationTransition * float32

type UnityLife(token: GameObject, reaper: GameObject, selectorCube: GameObject, pauseBetweenGenerations: float32, pauseWait: float32) =

    let mutable gameState = Running Time.time
    do selectorCube.SetActive(false)

    (******** PUBLIC MEMBERS **********************************************************************************)
    member this.RunGame (generationTransition: GenerationTransition): IEnumerator =
        let initialWait = seq { yield WaitForSeconds(pauseBetweenGenerations) }
        let waitSequence = generationSequence generationTransition
                            |> Seq.takeWhile (fun generationPair -> this.ShouldContinue(generationPair))
                            |> Seq.map (fun generationPair -> this.UpdateSceneAndWait(generationPair)) 
                            |> Seq.append initialWait

        waitSequence.GetEnumerator() :> IEnumerator

    member this.GameState = gameState

    member this.PauseToggled timeOfLastToggle =
        Input.GetKey(KeyCode.P) && pauseWaitElapsed timeOfLastToggle pauseWait Time.time

    member this.MaybeModifyPattern() =
        match gameState with
            | Paused((thisGeneration, nextGeneration), pauseStart) when this.SelectToggled pauseStart ->
                this.ToggleCellAt(selectorCube.gameObject.transform.position, thisGeneration) 
            | _ -> ()            

    (******** PRIVATE MEMBERS **********************************************************************************)
    member private this.ShouldContinue (generationPair: GenerationTransition) =
        match gameState with 
            | Running startTime when this.PauseToggled startTime ->
                gameState <- Paused(generationPair, Time.time)
                selectorCube.SetActive(true)
                false
            | Running startTime when Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.Escape) ->
                gameState <- Restarting
                selectorCube.SetActive(false)
                false
            | _ -> true

    member private this.UpdateSceneAndWait (generationTransition: GenerationTransition) =
        Set.iter this.DeathAt (cellsToDestroy generationTransition)
        Set.iter this.BirthAt (cellsToCreate generationTransition)
        WaitForSeconds(pauseBetweenGenerations)

    member private this.BirthAt (cell: Cell) : Unit =
        Object.Instantiate(token, (unityCoordinatesFrom cell), Quaternion.identity) |> ignore

    member private this.DeathAt (cell: Cell) : Unit =
        Object.Instantiate(reaper, (unityCoordinatesFrom cell), Quaternion.identity) |> ignore

    member private this.SelectToggled timeOfLastToggle =
        Input.GetKey(KeyCode.Space) && pauseWaitElapsed timeOfLastToggle pauseWait Time.time

    member private this.ToggleCellAt (selectorCoordinates: Vector3, thisGeneration: Generation) =
        let selectedCell = cellFromUnityCoordinates selectorCoordinates

        let newGeneration = 
            if Set.contains selectedCell thisGeneration
            then this.DeathAt selectedCell                                
                 Set.remove selectedCell thisGeneration
            else this.BirthAt selectedCell
                 Set.add selectedCell thisGeneration

        let newNextGeneration = nextGeneration newGeneration
        gameState <- Paused((newGeneration, newNextGeneration), Time.time)
