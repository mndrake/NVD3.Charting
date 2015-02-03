// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "bin/Debug/NVD3.Charting"
#r "bin/Debug/Newtonsoft.Json"

open NVD3.Charting
open Newtonsoft.Json

let series1 = { 
    key = "sin wave"
    color = "#ff7f0e"
    values = seq [ for i in 0. .. 100. -> {x = i; y = sin(i / 10.)}]
    }
    
let series2 = {
    key = "cos wave"
    color = "#2ca02c"
    values = seq [ for i in 0. .. 100. -> {x=i; y = 0.5 * cos(i / 10.)}]
    }

//let data = [| for i in 0. .. 10. -> i, i*i |]

//let json = 
//    data
//   |> Array.map (fun p -> { x = fst p; y = snd p })
//    |> JsonConvert.SerializeObject
