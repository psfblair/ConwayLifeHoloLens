namespace ConwayLife.Core

module Life =

    type Cell = int * int * int
    type Neighborhood = Set<Cell>
    type Neighbors = Set<Cell>
    type Generation = Set<Cell>

    type IsAlive = bool
    type LiveNeighborCount = int
    type CellState = Cell * IsAlive * LiveNeighborCount

    [<Literal>]
    let SurvivalLowerThreshold = 4
    [<Literal>]
    let SurvivalUpperThreshold = 5
    [<Literal>]
    let SpawnNeighborCount = 5

    [<assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ConwayLifeTests")>]
    do()

    let internal neighborhood ((a,b,c):Cell) : Neighborhood = 
        [   for i in -1..1 do
            for j in -1..1 do
            for k in -1..1 do
                yield a+i, b+j, c+k
        ] |> Set.ofList

    let internal neighbors (cell:Cell) : Neighbors = neighborhood cell |> Set.remove cell

    let internal cellState (liveCells:Generation) (cell:Cell) : CellState =
        let liveNeighborCount = neighbors cell |> Set.intersect liveCells |> Set.count
        let isCellAlive = liveCells |> Set.contains cell 
        (cell, isCellAlive, liveNeighborCount)

    let internal cellsRelevantToNextGeneration (currentGeneration:Generation) : Set<Cell> = 
        currentGeneration 
        |> Set.map neighborhood 
        |> Set.unionMany 

    let internal survives (cellState:CellState) : bool = 
        match cellState with
            | _, true,    SurvivalLowerThreshold -> true
            | _, true,    SurvivalUpperThreshold -> true
            | _, false,   SpawnNeighborCount     -> true
            | _                                  -> false

    let nextGeneration (currentGeneration:Generation) : Generation = 
        currentGeneration 
        |> cellsRelevantToNextGeneration 
        |> Set.map (cellState currentGeneration) 
        |> Set.filter survives 
        |> Set.map (fun (cell,_,_) -> cell)

    let allGenerations (initialGeneration:Generation): seq<Generation> =
        initialGeneration
        |> Seq.unfold (fun generation -> Some(generation, nextGeneration generation))
        
(*
Suppose instead we want to retain all the generations we generate, so we can jump to
any given generation given the generation number. The easiest way to get the generation 
for a given index is:

    let generationNumber (initialGeneration:Generation) (index: uint64): Generation =
        initialGeneration
        |> allGenerations 
        |> Seq.skip (int index) 
        |> Seq.head

However, this means that for every lookup we have to traverse the entire sequence of
generations up to that point to create the new one. We would prefer to cache the results
we have already calculated instead. (This is called memoization.)

What is the function we want to memoize? For any given initial generation, we want to
memoize the curried function:

    (generationNumber initialGeneration)

However, doing this with generationNumber as written above doesn't let us cache the 
intermediate values that we traverse. We want to use a recursive function instead,
and memoize each value traversed. The key here is that we work from the target index
backwards, creating at each level a function that will calculate and cache the value
from the next lower level, while then calling the function created at the next higher
nesting level. When we get to a value that is already cached (or to level zero) we
then apply to that value the accumulated nested function that will unfold from it the
values for all the levels above it up to the target index.

Note that memoizedGenerationNumber returns a *function* that contains a cache inside
itself. That function is of an unsigned long (uint64), which is the number of the
generation we are looking for. In other words, we call memoizedGenerationNumber to get 
the function we will call to get the generation for any given generation number.

    type GenerationNumber = uint64
    let zerothGenerationNumber = 0UL
    let generationNumberIncrement = 1UL

    let memoizedGenerationNumber initialGeneration =
        let cache = new System.Collections.Generic.SortedList<GenerationNumber, Generation>()

        let rec generationNumber index generateAndCacheResultsForNestingLevels =
            match cache.TryGetValue(index) with
            | true, cached -> generateAndCacheResultsForNestingLevels cached
            | _ -> 
                if index = zerothGenerationNumber then
                    generateAndCacheResultsForNestingLevels initialGeneration
                else
                    let generateAndCacheResultsForThisLevelAndAbove resultForPriorLevel =
                        let resultForThisLevel = nextGeneration resultForPriorLevel
                        cache.Add(index, resultForThisLevel)
                        generateAndCacheResultsForNestingLevels resultForThisLevel
                    generationNumber (index - generationNumberIncrement) generateAndCacheResultsForThisLevelAndAbove
        (fun index -> generationNumber index id)


If we wanted we could separate the above function into a more abstract memoize function 
that takes a function to be memoized. The disadvantage is that it's so abstract it's even
harder to understand. That would look like this:

    let memoize memoizableFunction cache =
        let rec memoizer key generateAndCacheResultsForNestingLevels = 
            match cache.TryGetValue(key) with
            | true, cached -> generateAndCacheResultsForNestingLevels cached
            | _ -> 
                // Function that we will pass to our memoizable function that will actually calculate the result
                let generateAndCacheResultsForThisLevelAndAbove resultForThisLevel =
                    cache.Add(key,resultForThisLevel)
                    generateAndCacheResultsForNestingLevels resultForThisLevel
                memoizableFunction key generateAndCacheResultsForThisLevelAndAbove memoizer
        (fun key -> memoizer key id)


    let memoizableGenerationNumber index cacheResultsForThisLevelAndGenerateResultsForNestingLevels memoizer =
        if index = 0UL then
            cacheResultsForThisLevelAndGenerateResultsForNestingLevels initialGeneration
        else
            // Function that we will pass to our memoizer for the next level down
            let generateAndCacheResultsForThisAndNestingLevels priorGeneration =
                let resultForThisLevel = nextGeneration priorGeneration
                cacheResultsForThisLevelAndGenerateResultsForNestingLevels resultForThisLevel
            memoizer (n - 1UL) generateAndCacheResultsForThisAndNestingLevels


    let memoizedGenerationNumber = 
        let cache = new System.Collections.Generic.SortedList<uint64, Generation>()
        memoize memoizableGenerationNumber cache 
*)    