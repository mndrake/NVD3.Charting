// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "bin/Debug/NVD3.Charting"
#r "bin/Debug/Newtonsoft.Json"

open NVD3.Charting
open Newtonsoft.Json

let data = [| for i in 0. .. 10. -> i, i*i |]

let json = 
    data
    |> Array.map (fun p -> { x = fst p; y = snd p })
    |> JsonConvert.SerializeObject
