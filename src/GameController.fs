namespace ConwayLife3D.Controllers

open UnityEngine
open UnityEngine.UI
open ConwayLife3D
open ConwayLife3D.Patterns
open ConwayLife3D.Unity
open ConwayLife3D.Life.Core
open System.Collections

type GameController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable pauseBetweenGenerations: float32 = 1.0f

    [<SerializeField>] 
    let mutable pauseWait: float32 = 1.0f

    [<SerializeField>] [<DefaultValue>] val mutable token: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable reaper: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable startPanel: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable restartText: Text
    [<SerializeField>] [<DefaultValue>] val mutable runner: UnityLife

    let restartMessage = "Press 'R' to Restart"

    member this.Start () =
        this.startPanel.gameObject.SetActive(true)
        this.restartText.text <- restartMessage

    // Called on button click
    member this.StartGame (patternName: string) =
        this.startPanel.gameObject.SetActive(false)
        this.restartText.text <- ""
        let selectedPattern = this.GetSelectedPattern (patternName)
        this.RunUnityLife (Set.empty, selectedPattern)

    member this.Update () =
        match this.runner.GameState with
            | Paused(generationPair, pauseStart) when this.runner.PauseToggled(pauseStart) -> this.RunUnityLife generationPair
            | Restarting -> Application.LoadLevel(Application.loadedLevel)
            | _ -> ()

    member private this.GetSelectedPattern (patternName: string): Generation =
        let patternsType = typeof<PatternModuleTypeAccessor>.DeclaringType
        patternsType.GetProperty(patternName).GetValue(null, Array.empty) :?> Generation

    member private this.RunUnityLife (generationPair: GenerationTransition) = 
        this.runner <- new UnityLife(this.token, this.reaper, pauseBetweenGenerations, pauseWait)
        this.StartCoroutine (this.runner.RunGame(generationPair)) |> ignore
