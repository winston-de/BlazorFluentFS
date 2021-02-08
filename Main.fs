module BlazorFluentFS.Main

open Elmish
open Bolero
open Bolero.Html
open BlazorFluentUI
open Microsoft.JSInterop
open Microsoft.AspNetCore.Components
open System.Threading.Tasks
open System
open System.Diagnostics

type Model =
    { x: string
      Count: int
      MenuItemPressed: string }

let initModel =
    { x = ""
      Count = 0
      MenuItemPressed = "" }

type Message =
    | Ping
    | IncrementCount
    | SetPressedMenuItem of string

let update message model =
    match message with
    | Ping -> model
    | IncrementCount -> { model with Count = model.Count + 1 }
    | SetPressedMenuItem item -> { model with MenuItemPressed = item }

let view model dispatch =
    comp<BFUTheme>
        []
        [ comp<BFULayerHost>
              []
              [ body [] [
                    comp<BFUPrimaryButton>
                        [ "Text" => "Counter"
                          on.click (fun _ -> dispatch IncrementCount) ]
                        []
                    text $"Clicked {model.Count.ToString()} times"
                    comp<BFUPrimaryButton>
                        [ "Text" => "Open menu"
                          "MenuItems"
                          => [ BFUContextualMenuItem(
                                   Text = "This is a menu item",
                                   Key = "Num1",
                                   OnClick = (fun arg -> dispatch (SetPressedMenuItem arg.Key))
                               )
                               BFUContextualMenuItem(
                                   Text = "Oh look another",
                                   Key = "Num2",
                                   OnClick = (fun arg -> dispatch (SetPressedMenuItem arg.Key))
                               )
                               BFUContextualMenuItem(
                                   Text = "This one has submenus",
                                   Key = "Num3",
                                   Items =
                                       [ BFUContextualMenuItem(
                                           Text = "I'm a submenu!",
                                           Key = "Sub1",
                                           OnClick = (fun arg -> dispatch (SetPressedMenuItem arg.Key))
                                         )
                                         BFUContextualMenuItem(
                                             Text = "I'm another submenu!",
                                             Key = "Sub2",
                                             OnClick = (fun arg -> dispatch (SetPressedMenuItem arg.Key))
                                         ) ]
                               ) ] ]
                        []
                    text $"{model.MenuItemPressed} Clicked"
                ] ] ]

let UpdateTheme (useDarkMode: bool, themeProvider: ThemeProvider) =
    if useDarkMode then
        let palette = DefaultPaletteDark()
        themeProvider.UpdateTheme(palette, DefaultSemanticColorsDark(palette), DefaultSemanticTextColorsDark(palette))
    else
        themeProvider.UpdateTheme(DefaultPalette())

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    [<Inject>]
    member val ThemeProvider: ThemeProvider = Unchecked.defaultof<ThemeProvider> with get, set

    static member val Action = (fun (v: bool) -> printf $"{v}") with get, set

    override this.Program =
        Program.mkSimple (fun _ -> initModel) update view

    override this.OnInitializedAsync() =
        MyApp.Action <- (fun (v: bool) -> UpdateTheme(v, this.ThemeProvider))

        async {
            let! result =
                this
                    .JSRuntime
                    .InvokeAsync<bool>("isDarkTheme")
                    .AsTask()
                |> Async.AwaitTask<bool>

            UpdateTheme(result, this.ThemeProvider)
        }
        |> Async.Start

        base.OnInitializedAsync()

    /// Called by the JavaScript when the browsers theme is changed
    [<JSInvokable>]
    static member ChangeThemeCaller v = MyApp.Action(v)
