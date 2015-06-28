namespace ConwayLife3D.Controllers

open UnityEngine
open UnityEngine.UI
open ConwayLife3D
open ConwayLife3D.Patterns
open ConwayLife3D.Life.Core
open System.Collections

type GenerationTransition = Generation * Generation
type GameState = 
    | Running 
    | Restarting
    | Paused of GenerationTransition * float32

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

    let mutable gameState = Running

    let restartMessage = "Press 'R' to Restart"

    let nextGenerationPairedWithPrevious (previousGeneration, generation): GenerationTransition  =
        let nextGen = nextGeneration generation
        (generation, nextGen)
        
    let generationSequence (firstGenerationPair: GenerationTransition): seq<GenerationTransition> =
        Seq.unfold (fun generationPair -> Some(generationPair, nextGenerationPairedWithPrevious generationPair)) firstGenerationPair

    let unityCoordinatesFrom (cell: Cell): Vector3 = 
        match cell with
            | (x, y, z)     -> new Vector3(float32 x, float32 y, float32 z)

    member this.Start () =
        gameState <- Running
        this.startPanel.gameObject.SetActive(true)
        this.restartText.text <- restartMessage

    member this.Update () =
        match gameState with
            | Restarting ->  Application.LoadLevel(Application.loadedLevel)
            | Paused(generationPair, pauseStart) when this.PauseWaitElapsed(pauseStart) && Input.GetKey(KeyCode.P) ->
                Debug.Log("Continuing")
                gameState <- Running
                this.StartCoroutine (this.RunGame generationPair) |> ignore
            | _ -> ()

    member this.StartGame (patternName: string) =
        this.startPanel.gameObject.SetActive(false)
        this.restartText.text <- ""
        let patternsType = typeof<PatternModuleTypeAccessor>.DeclaringType
        let selectedPattern = patternsType.GetProperty(patternName).GetValue(null, Array.empty) :?> Generation
        this.StartCoroutine (this.RunGame (Set.empty, selectedPattern)) |> ignore

    member private this.RunGame (generationTransition: GenerationTransition): IEnumerator =
        let initialWait = seq { yield WaitForSeconds(pauseBetweenGenerations) }
        let waitSequence = generationSequence generationTransition
                            |> Seq.takeWhile (fun generationPair -> this.ShouldContinue(generationPair))
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

    member private this.ShouldContinue (generationPair: GenerationTransition) =
        match gameState with 
            | Running when Input.GetKey(KeyCode.P) ->
                Debug.Log("Pausing")
                gameState <- Paused(generationPair, Time.time)
                false
            | Running when Input.GetKey(KeyCode.R) ->
                Debug.Log("Stopping")
                gameState <- Restarting
                false
            | _ -> true

    member private this.PauseWaitElapsed pauseStart = pauseStart + pauseWait < Time.time