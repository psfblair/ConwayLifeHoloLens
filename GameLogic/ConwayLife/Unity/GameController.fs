namespace ConwayLife.Unity

open UnityEngine
open System.Collections

open ConwayLife.Core

type GameController() = 
    inherit MonoBehaviour()    

    [<SerializeField>] [<DefaultValue>] val mutable token: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable scalingFactor: float32
    [<SerializeField>] [<DefaultValue>] val mutable secondsBetweenGenerations: float32

    let mutable presentAndFutureGenerations: seq<Life.Generation> = Seq.ofList [ Set.empty ]

    member this.Start () =
        let initialGeneration = Patterns.BLINKER_P2
        let lifeUI = UnityLifeUI.lifeUI this.token this.gameObject.transform this.scalingFactor
        presentAndFutureGenerations <- Life.allGenerations initialGeneration
        Game.initialize lifeUI initialGeneration |> ignore
        this.AdvanceGame(lifeUI) |> this.StartCoroutine |> ignore

    member this.AdvanceGame (lifeUI: LifeUI): IEnumerator =
        let advanceAndWaitSequence = () |> Seq.unfold (fun () ->
            presentAndFutureGenerations <- Game.advance lifeUI presentAndFutureGenerations
            Some(WaitForSeconds(this.secondsBetweenGenerations), ())
        ) 
        let initialPause = Seq.singleton <| WaitForSeconds(this.secondsBetweenGenerations)
        let entireSequence = Seq.append initialPause advanceAndWaitSequence
        entireSequence.GetEnumerator() :> IEnumerator