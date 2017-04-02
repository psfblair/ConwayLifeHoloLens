namespace ConwayLife.Core

open ConwayLife.Core

type GenerationDelta = { births: Set<Life.Cell>; deaths: Set<Life.Cell> }

type LifeUI = { giveBirthAt: Life.Cell -> unit; dieAt: Life.Cell -> unit }

module Game =

    let internal delta (previousGeneration: Life.Generation) (nextGeneration: Life.Generation): GenerationDelta =
        { births = nextGeneration - previousGeneration
        ; deaths = previousGeneration - nextGeneration
        }

    let internal displayDelta (ui: LifeUI) (delta: GenerationDelta): unit =
        Set.iter ui.giveBirthAt delta.births
        Set.iter ui.dieAt delta.deaths
        ()

    let initialize (ui: LifeUI) (initialGeneration: Life.Generation): Life.Generation =
        let deltaFromNothing = delta Set.empty initialGeneration
        displayDelta ui deltaFromNothing
        initialGeneration

    let advance (ui: LifeUI) (generations: seq<Life.Generation>): seq<Life.Generation> =
        let previousGeneration = generations |> Seq.head
        let futureGenerations = generations |> Seq.skip 1
        let newGeneration = futureGenerations |> Seq.head
        let delta = delta previousGeneration newGeneration
        displayDelta ui delta
        futureGenerations