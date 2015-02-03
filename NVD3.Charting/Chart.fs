module NVD3.Charting
open System
open Newtonsoft.Json

type Point = {x: float; y:float}
type Series = {values: Point seq; key: string; color: string}

let template = """
<div id="{GUID}" class="container"><svg style='height:{HEIGHT}px;width:{WIDTH}px'></svg></div>
<link rel="stylesheet" type="text/css" href="//cdnjs.cloudflare.com/ajax/libs/nvd3/1.1.15-beta/nv.d3.min.css">
<script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/require.js/2.1.15/require.min.js"></script>
<script type="text/javascript">
    require.config({
        paths: {d3: "//cdnjs.cloudflare.com/ajax/libs/d3/3.5.3/d3.min",
                nvd3: "//cdnjs.cloudflare.com/ajax/libs/nvd3/1.1.15-beta/nv.d3.min"}});
    require(["d3", "nvd3"], function(d3, nvd3) {
                nv.addGraph(function() {
                    var chart = nv.models.lineChart()
                                  .margin({left: 100})
                                  .transitionDuration(350)
                                  ;
                    {XAXIS}
                    {YAXIS}
                    d3.select('#{GUID} svg')
                            .datum({DATA})
                            .transition().duration(500)
                            .call(chart);
                    nv.utils.windowResize(chart.update);
                    return chart;});});
</script>"""

type Options = { 
    xLabel : string option
    xTickFormat : string
    yLabel : string option
    yTickFormat : string
    width : int
    height : int
    }
    with 
    static member New() = {
        xLabel = None
        xTickFormat = "0.1f"
        yLabel = None
        yTickFormat = "0.1f"
        width = 600
        height = 400
        }    

type GenericChart = {
    guid: string
    datum : Series seq
    options : Options
    }

type NChart =

    static member New() =
        { guid = String.Format("N{0}", Guid.NewGuid().ToString())
          datum = Seq.empty
          options = Options.New()
          }

    static member Line (values: seq<float*float>) =
        { NChart.New() with 
            datum = [{ values = values |> Seq.map (fun p -> {x = fst p; y = snd p }) 
                       key = "Series 1"
                       color = "#ff7f0e" }] }

    static member Line (values: Series) =
        { guid = String.Format("N{0}", Guid.NewGuid().ToString())
          datum = seq [values]
          options = Options.New()
          }

    static member AddSeries (values: Series) (chart: GenericChart) =
        { chart with datum = Seq.append chart.datum [values] }

    static member toHtml (chart: GenericChart) =
        let xAxis = 
            let label = match chart.options.xLabel with
                        | Some v -> String.Format(".axisLabel('{0}')", v)
                        | None   -> ""
            String.Format("chart.xAxis{0}.tickFormat(d3.format('{1}'));", label, chart.options.xTickFormat)
        let yAxis = 
            let label = match chart.options.yLabel with
                        | Some v -> String.Format(".axisLabel('{0}')", v)
                        | None   -> ""
            String.Format("chart.yAxis{0}.tickFormat(d3.format('{1}'));", label, chart.options.yTickFormat)
        template.Replace("{GUID}", chart.guid)
            .Replace("{DATA}", (chart.datum |> JsonConvert.SerializeObject))
            .Replace("{WIDTH}", string chart.options.width)
            .Replace("{HEIGHT}", string chart.options.height)
            .Replace("{XAXIS}", xAxis)
            .Replace("{YAXIS}", yAxis)        

    static member xLabel value (chart: GenericChart) =
        { chart with options = { chart.options with xLabel = Some value }}

    static member yLabel value (chart: GenericChart) =
        { chart with options = { chart.options with yLabel = Some value }}

    static member xTickFormat value (chart: GenericChart) =
        { chart with options = { chart.options with xTickFormat = value }}

    static member yTickFormat value (chart: GenericChart) =
        { chart with options = { chart.options with yTickFormat = value }}

    static member height value (chart: GenericChart) =
        { chart with options = { chart.options with height = value }}

    static member width value (chart: GenericChart) =
        { chart with options = { chart.options with width = value }}