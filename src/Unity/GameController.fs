﻿namespace ConwayLife3D.Controllers

open ConwayLife3D
open ConwayLife3D.Life.Core
open ConwayLife3D.Life.Game
open ConwayLife3D.Patterns
open ConwayLife3D.Unity

open UnityEngine
open UnityEngine.UI
open System.Collections

type GameController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable pauseBetweenGenerations: float32 = 1.0f

    [<SerializeField>] 
    let mutable pauseWait: float32 = 1.0f

    [<SerializeField>] [<DefaultValue>] val mutable token: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable reaper: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable explanationPanel: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable buttonPanel: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable instructionsPanel: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable selectorCube: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable runner: UnityLife

    (******** PUBLIC MEMBERS **********************************************************************************)
    member this.Start () =
        this.SetControlPanelState (true)
        this.selectorCube.SetActive (false)

    // Called on button click
    member this.StartGame (patternName: string) =
        this.SetControlPanelState (false)
        let selectedPattern = this.GetSelectedPattern (patternName)
        this.RunUnityLife (Set.empty, selectedPattern)

    member this.Update () =
        match this.runner.GameState with
            | Paused(generationPair, pauseStart) when this.runner.PauseToggled(pauseStart) -> this.RunUnityLife generationPair
            | Paused(_, _) -> this.runner.MaybeModifyPattern()
            | Restarting -> Application.LoadLevel(Application.loadedLevel)
            | _ -> ()

    (******** PRIVATE MEMBERS **********************************************************************************)
    member private this.GetSelectedPattern (patternName: string): Generation =
        let patternsType = typeof<PatternModuleTypeAccessor>.DeclaringType
        patternsType.GetProperty(patternName).GetValue(null, Array.empty) :?> Generation

    member private this.RunUnityLife (generationPair: GenerationTransition) = 
        this.runner <- new UnityLife(this.token, this.reaper, this.selectorCube, pauseBetweenGenerations, pauseWait)
        this.StartCoroutine (this.runner.RunGame(generationPair)) |> ignore

    member private this.SetControlPanelState (state: bool) =
        this.explanationPanel.gameObject.SetActive(state)
        this.buttonPanel.gameObject.SetActive(state)
        this.instructionsPanel.gameObject.SetActive(state)
