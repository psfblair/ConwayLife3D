namespace ConwayLife3D.Life

open ConwayLife3D.Life.Core

module Game =
    type GenerationTransition = Generation * Generation

    let nextGenerationPairedWithPrevious (previousGeneration, generation): GenerationTransition =
        let nextGen = nextGeneration generation
        (generation, nextGen)

    let cellsToDestroy = function
        | (previousGeneration: Generation, thisGeneration: Generation) -> previousGeneration - thisGeneration

    let cellsToCreate = function
        | (previousGeneration: Generation, thisGeneration: Generation) ->  thisGeneration - previousGeneration

    let generationSequence (firstGenerationPair: GenerationTransition): seq<GenerationTransition> =
        Seq.unfold (fun generationPair -> Some(generationPair, nextGenerationPairedWithPrevious generationPair)) firstGenerationPair

