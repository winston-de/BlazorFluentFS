namespace BlazorFluentFS

open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Bolero.Remoting.Client
open BlazorFluentUI

module Program =

    [<EntryPoint>]
    let Main args =
        let builder = WebAssemblyHostBuilder.CreateDefault(args)
        builder.RootComponents.Add<Main.MyApp>("#main")
        builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
        builder.Services.AddBlazorFluentUI()
        builder.Build().RunAsync() |> ignore
        0
