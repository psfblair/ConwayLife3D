namespace ConwayLife3D.Life

module Core =

   type Cell = int * int * int
   type Neighborhood = Set<Cell>
   type Neighbors = Set<Cell>
   type Generation = Set<Cell>

   type IsAlive = bool
   type LiveNeighborCount = int
   type CellState = Cell * IsAlive * LiveNeighborCount

   let neighborhood ((a,b,c):Cell) : Neighborhood = 
       [   for i in -1..1 do
           for j in -1..1 do
           for k in -1..1 do
               yield a+i, b+j, c*k
       ] |> Set.ofList

   let neighbors (cell:Cell) : Neighbors = neighborhood cell |> Set.remove cell

   let cellState (liveCells:Generation) (cell:Cell) : CellState =
       let liveNeighborCount = neighbors cell |> Set.intersect liveCells |> Set.count
       let isCellAlive = liveCells |> Set.contains cell 
       (cell, isCellAlive, liveNeighborCount)

   let survives (cellState:CellState) : bool = 
       match cellState with
           | _, _,    5 -> true
           | _, true, 4 -> true
           | _          -> false

   let nextGeneration (livingCells:Generation) : Generation = 
       livingCells |> Set.map neighborhood 
                   |> Set.unionMany 
                   |> Set.map (cellState livingCells) 
                   |> Set.filter survives 
                   |> Set.map (fun (cell,_,_) -> cell)

