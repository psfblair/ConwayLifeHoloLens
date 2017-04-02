namespace ConwayLifeTests.Core

open NUnit.Framework
open Swensen.Unquote

open ConwayLife.Core

type FakeUIHolder() = 

    member val Births = Set.empty with get, set
    member val Deaths = Set.empty with get, set

    member this.FakeUI = 
        { giveBirthAt = fun cell -> this.Births <- Set.add cell this.Births
        ; dieAt = fun cell -> this.Deaths <- Set.add cell this.Deaths
        }

module GameTests =

    [<Test>]
    let ``Delta of two empty generations is empty``() = 
        Game.delta Set.empty Set.empty =! { births = Set.empty; deaths = Set.empty }

    [<Test>]
    let ``Delta from nothing to one living cell is the birth of that cell``() = 
        let singleLivingCellGeneration = Set.ofList [ (0,0,0) ]
        Game.delta Set.empty singleLivingCellGeneration =! { births = Set.ofList [ (0,0,0) ]; deaths = Set.empty }

    [<Test>]
    let ``Delta from one living cell to nothing is the death of that cell``() = 
        let singleLivingCellGeneration = Set.ofList [ (0,0,0) ]
        Game.delta singleLivingCellGeneration Set.empty  =! { births = Set.empty; deaths = Set.ofList [ (0,0,0) ] }

    [<Test>]
    let ``Delta of two generations of a stable pattern is nothing``() = 
        let generationTwo = Patterns.CROSS |> Life.nextGeneration
        Game.delta Patterns.CROSS generationTwo =! { births = Set.empty; deaths = Set.empty } 

    [<Test>]
    let ``Delta of the period of a periodic pattern is nothing``() = 
        let generationThree = Patterns.BLINKER_P2 |> Life.nextGeneration |> Life.nextGeneration
        Game.delta Patterns.BLINKER_P2 generationThree =! { births = Set.empty; deaths = Set.empty } 

    [<Test>]
    let ``Delta for the first generation of a blinker should have two births and two deaths``() = 
        let expectedBirths = Set.ofList [ (0,  -1,  1);  ( 0,  1,  1) ] 
        let expectedDeaths = Set.ofList [ (-1,  0,  1);  ( 1,  0,  1) ] 
        let generationThree = Patterns.BLINKER_P2 |> Life.nextGeneration
        Game.delta Patterns.BLINKER_P2 generationThree =! { births = expectedBirths; deaths = expectedDeaths } 

    [<Test>]
    let ``Initializes empty game``() = 
        let fakeUIHolder = FakeUIHolder()
        test <@
                Game.initialize fakeUIHolder.FakeUI Set.empty = Set.empty &&
                fakeUIHolder.Births = Set.empty &&
                fakeUIHolder.Deaths = Set.empty
             @>

    [<Test>]
    let ``Initializes game from nothing``() = 
        let fakeUIHolder = FakeUIHolder()
        test <@
                Game.initialize fakeUIHolder.FakeUI Patterns.CROSS = Patterns.CROSS &&
                fakeUIHolder.Births = Patterns.CROSS &&
                fakeUIHolder.Deaths = Set.empty
             @>

    [<Test>]
    let ``Advances an empty pattern from the initial generation``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerations = Life.allGenerations Set.empty
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerations |> Seq.head = Set.empty &&
                fakeUIHolder.Births = Set.empty &&
                fakeUIHolder.Deaths = Set.empty
             @>

    [<Test>]
    let ``Advances an empty pattern from generation 2``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerationsAfterFirst = Life.allGenerations Set.empty |> Seq.skip 1
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerationsAfterFirst |> Seq.head = Set.empty &&
                fakeUIHolder.Births = Set.empty &&
                fakeUIHolder.Deaths = Set.empty
             @>

    [<Test>]
    let ``Advances a stable pattern from the initial generation``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerations = Life.allGenerations Patterns.CROSS
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerations |> Seq.head = Patterns.CROSS &&
                fakeUIHolder.Births = Set.empty &&
                fakeUIHolder.Deaths = Set.empty
             @>

    [<Test>]
    let ``Advances a stable pattern from generation 3``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerationsAfterSecond = Life.allGenerations Patterns.CROSS|> Seq.skip 2
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerationsAfterSecond |> Seq.head = Patterns.CROSS &&
                fakeUIHolder.Births = Set.empty &&
                fakeUIHolder.Deaths = Set.empty
             @>

    [<Test>]
    let ``Advances a P2 pattern from the initial generation``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerations = Life.allGenerations Patterns.BLINKER_P2
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerations |> Seq.head <> Patterns.BLINKER_P2 &&
                fakeUIHolder.Births = Set.ofList [ (0,  -1,  1);  ( 0,  1,  1) ] &&
                fakeUIHolder.Deaths = Set.ofList [ (-1,  0,  1);  ( 1,  0,  1) ] 
             @>

    [<Test>]
    let ``Advances a P2 pattern from the second generation``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerationsAfterFirst = Life.allGenerations Patterns.BLINKER_P2 |> Seq.skip 1
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerationsAfterFirst |> Seq.head = Patterns.BLINKER_P2 &&
                fakeUIHolder.Births = Set.ofList [ (-1,  0,  1);  ( 1,  0,  1) ] && 
                fakeUIHolder.Deaths = Set.ofList [ (0,  -1,  1);  ( 0,  1,  1) ] 
             @>

    [<Test>]
    let ``Advances a P2 pattern from an odd generation - should be same as transition from the first generation``() = 
        let fakeUIHolder = FakeUIHolder()
        let secondGeneration = Life.allGenerations Patterns.BLINKER_P2 |> Seq.skip 1 |> Seq.head
        // Note that the current generation is included, so e.g., skip 2 puts you back to an odd generation
        // at the head of the sequence (i.e., from the first, a generation where the third is the head)
        let allGenerationsAfterSixth = Life.allGenerations Patterns.BLINKER_P2 |> Seq.skip 6
        test <@
                // Seventh generation is now the head, i.e., the previous generation, so the delta will
                // be between the seventh and the eighth generations
                Game.advance fakeUIHolder.FakeUI allGenerationsAfterSixth |> Seq.head = secondGeneration &&
                fakeUIHolder.Births = Set.ofList [ (0,  -1,  1);  ( 0,  1,  1) ] &&
                fakeUIHolder.Deaths = Set.ofList [ (-1,  0,  1);  ( 1,  0,  1) ] 
             @>

    [<Test>]
    let ``Advances a P2 pattern from an even generation - should be same as from the second generation``() = 
        let fakeUIHolder = FakeUIHolder()
        let allGenerationsAfterSeventeenth = Life.allGenerations Patterns.BLINKER_P2 |> Seq.skip 17
        test <@
                Game.advance fakeUIHolder.FakeUI allGenerationsAfterSeventeenth |> Seq.head = Patterns.BLINKER_P2 &&
                fakeUIHolder.Births = Set.ofList [ (-1,  0,  1);  ( 1,  0,  1) ] && 
                fakeUIHolder.Deaths = Set.ofList [ (0,  -1,  1);  ( 0,  1,  1) ] 
             @>
