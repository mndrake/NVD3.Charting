module NVD3.Charting
open System
open Newtonsoft.Json

type Point = {x: float; y:float}
type Series = {values: Point seq; key: string; color: string}
type Axis = {label: string option; format: string option}

let template = """
<svg id="{GUID}" style='height:{HEIGHT}px;width:{WIDTH}px'></svg>
<link rel="stylesheet" type="text/css" href="//cdnjs.cloudflare.com/ajax/libs/nvd3/1.1.15-beta/nv.d3.min.css">
<script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/require.js/2.1.15/require.min.js"></script>
<script type="text/javascript">
    require.config({
        paths: {d3: "//cdnjs.cloudflare.com/ajax/libs/d3/3.5.3/d3.min",
                nvd3: "//cdnjs.cloudflare.com/ajax/libs/nvd3/1.1.15-beta/nv.d3.min"}});
    require(["d3", "nvd3"], function(d3, nvd3) {
                nv.addGraph(function() {
                    var chart = nv.models.lineChart().useInteractiveGuideline(true);
                    {XAXIS}
                    {YAXIS}
                    d3.select('#{GUID}')
                            .datum({DATA})
                            .transition().duration(500)
                            .call(chart);
                    nv.utils.windowResize(chart.update);
                    return chart;});});
</script>"""

type NChart = 
    {
        guid: string
        xAxis: Axis option
        yAxis: Axis option
        height: int
        width: int
        datum: Series seq 
    }

    with  

    static member Line(values: seq<float*float>) = 
        { guid = "N" + System.Guid.NewGuid().ToString("N")
          xAxis = Some {label = None; format = Some "0.1f"}
          yAxis = Some {label = None; format = Some "0.1f"}
          height = 400
          width = 600
          datum = 
              [{ values = values |> Seq.map (fun p -> {x = fst p; y = snd p }) 
                 key = "Series 1"
                 color = "#ff7f0e" }]
         }

    member __.toHtml() =
        let axisFormat (axis:Axis) name =
            let label = match axis.label with
                        | Some v -> String.Format(".axisLabel('{0}'", v)
                        | None   -> ""
            let tickFormat = match axis.format with
                             | Some v -> String.Format(".tickFormat(d3.format('{0}'))", v)
                             | None   -> ""
            String.Format("chart.{0}{1}{2}", name, label, tickFormat)
        let xAxis = match __.xAxis with |None -> "" |Some v -> "xAxis" |> axisFormat v
        let yAxis = match __.yAxis with |None -> "" |Some v -> "yAxis" |> axisFormat v
        template.Replace("{GUID}", __.guid)
            .Replace("{DATA}", (__.datum |> JsonConvert.SerializeObject))
            .Replace("{WIDTH}", string __.width)
            .Replace("{HEIGHT}", string __.height)
            .Replace("{XAXIS}", xAxis)
            .Replace("{YAXIS}", yAxis)
