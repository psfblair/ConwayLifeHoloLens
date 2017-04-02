namespace ConwayLifeTests.Core

open NUnit.Framework
open Swensen.Unquote

open ConwayLife.Core

module LifeTests = 

    [<Test>]
    let ``Finds the neighborhood of a cell``() = 
        let expectedNeighborhood = Set.ofList [ (2,1, -3);   (2,1,-2);   (2,1,-1);
                                                (2,0, -3);   (2,0,-2);   (2,0,-1); 
                                                (2,-1,-3);  (2,-1,-2);  (2,-1,-1);
                                                (1,1, -3);   (1,1,-2);   (1,1,-1);
                                                (1,0, -3);   (1,0,-2);   (1,0,-1); 
                                                (1,-1,-3);  (1,-1,-2);  (1,-1,-1);
                                                (0,1, -3);   (0,1,-2);   (0,1,-1);
                                                (0,0, -3);   (0,0,-2);   (0,0,-1); 
                                                (0,-1,-3);  (0,-1,-2);  (0,-1,-1) ]

        Life.neighborhood (1,0,-2) =! expectedNeighborhood

    [<Test>]
    let ``Finds the neighbors of a cell``() = 
        let expectedNeighbors = Set.ofList [(2,1, -3);   (2,1,-2);   (2,1,-1);
                                            (2,0, -3);   (2,0,-2);   (2,0,-1); 
                                            (2,-1,-3);  (2,-1,-2);  (2,-1,-1);
                                            (1,1, -3);   (1,1,-2);   (1,1,-1);
                                            (1,0, -3);               (1,0,-1); 
                                            (1,-1,-3);  (1,-1,-2);  (1,-1,-1);
                                            (0,1, -3);   (0,1,-2);   (0,1,-1);
                                            (0,0, -3);   (0,0,-2);   (0,0,-1); 
                                            (0,-1,-3);  (0,-1,-2);  (0,-1,-1) ]

        Life.neighbors (1,0,-2) =! expectedNeighbors

    [<Test>]
    let ``Finds the state of a living cell``() = 
        let generation = Set.ofList [(2,1, -3);   (2,1,-2);   (2,1,-1);
                                     (2,0, -3);   (2,0,-2);   (2,0,-1); 
                                     (2,-1,-3);  (2,-1,-2);  (2,-1,-1);
                                     (1,1, -3);   (1,1,-2);   (1,1,-1);
                                                  (1,0,-2);
                                     (1,-1,-3);  (1,-1,-2);  (1,-1,-1);
                                     (0,1, -3);   (0,1,-2);   (0,1,-1);
                                     (0,0, -3);   (0,0,-2);   (0,0,-1); 
                                     (0,-1,-3);  (0,-1,-2);  (0,-1,-1) ]

        Life.cellState generation (1, 0, -2) =! ((1, 0, -2), true, 24)

    [<Test>]
    let ``Finds the state of a nonliving cell``() = 
        let generation = Set.ofList [(2,1, -3);   (2,1,-2);   (2,1,-1);
                                     (2,0, -3);   (2,0,-2);   (2,0,-1); 
                                     (2,-1,-3);  (2,-1,-2);  (2,-1,-1);
                                     (1,1, -3);   (1,1,-2);   (1,1,-1);
                                     (1,-1,-3);  (1,-1,-2);  (1,-1,-1);
                                     (0,1, -3);   (0,1,-2);   (0,1,-1);
                                     (0,0, -3);   (0,0,-2);   (0,0,-1); 
                                     (0,-1,-3);  (0,-1,-2);  (0,-1,-1) ]

        Life.cellState generation (1, 0, -2) =! ((1, 0, -2), false, 24)

    [<Test>]
    let ``Dies of loneliness with fewer neighbors than lower threshold``() = 
        let cell = (0,0,0)
        let isAlive = true
        let liveNeighborCount = ConwayLife.Core.Life.SurvivalLowerThreshold - 1
        Life.survives (cell, isAlive, liveNeighborCount) =! false

    [<Test>]
    let ``Survives with number of neighbors equal to lower threshold``() = 
        let cell = (0,0,0)
        let isAlive = true
        let liveNeighborCount = ConwayLife.Core.Life.SurvivalLowerThreshold
        Life.survives (cell, isAlive, liveNeighborCount) =! true

    [<Test>]
    let ``Dies of overcrowding with more neighbors than upper threshold``() = 
        let cell = (0,0,0)
        let isAlive = true
        let liveNeighborCount = ConwayLife.Core.Life.SurvivalUpperThreshold + 1
        Life.survives (cell, isAlive, liveNeighborCount) =! false

    [<Test>]
    let ``Survives with number of neighbors equal to upper threshold``() = 
        let cell = (0,0,0)
        let isAlive = true
        let liveNeighborCount = ConwayLife.Core.Life.SurvivalLowerThreshold
        Life.survives (cell, isAlive, liveNeighborCount) =! true

    [<Test>]
    let ``Has a birth with number of neighbors equal to spawn count``() = 
        let cell = (0,0,0)
        let isAlive = false
        let liveNeighborCount = ConwayLife.Core.Life.SpawnNeighborCount
        Life.survives (cell, isAlive, liveNeighborCount) =! true

    [<Test>]
    let ``Has no birth with number of neighbors greater than spawn count``() = 
        let cell = (0,0,0)
        let isAlive = false
        let liveNeighborCount = ConwayLife.Core.Life.SpawnNeighborCount + 1
        Life.survives (cell, isAlive, liveNeighborCount) =! false

    [<Test>]
    let ``Has no birth with number of neighbors less than spawn count``() = 
        let cell = (0,0,0)
        let isAlive = false
        let liveNeighborCount = ConwayLife.Core.Life.SpawnNeighborCount - 1
        Life.survives (cell, isAlive, liveNeighborCount) =! false

    [<Test>]
    let ``The next generation if only one cell is alive, is empty``() = 
        let generation = Set.ofList [ (0,0,0) ]
        Life.nextGeneration generation =! Set.empty

    [<Test>]
    let ``The next generation of a stable pattern is the same pattern``() = 
        Life.nextGeneration Patterns.CROSS =! Patterns.CROSS

    [<Test>]
    let ``The next generation of a P2 pattern should not be the same pattern``() = 
        Life.nextGeneration Patterns.BLINKER_P2 <>! Patterns.BLINKER_P2

    [<Test>]
    let ``The second generation of a P2 pattern should be the same pattern``() = 
        Life.nextGeneration Patterns.BLINKER_P2 |> Life.nextGeneration =! Patterns.BLINKER_P2

    [<Test>]
    let ``All generations of a stable pattern are the same``() = 
        Life.allGenerations Patterns.CROSS |> Seq.skip 50 |> Seq.head =! Patterns.CROSS

    [<Test>]
    let ``All generations of a repeating pattern alternate``() = 
        Life.allGenerations Patterns.BLINKER_P2 |> Seq.skip 2 |> Seq.head =! Patterns.BLINKER_P2
