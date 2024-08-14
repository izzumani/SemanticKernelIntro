
using semantic_kernel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Plugins;


public class MyProgram
{
    private static Kernel kernel = null;
    public  static void Main()
    {

        var (apiKey, orgId) = Setting.LoadFromFile();

        kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-3.5-turbo", apiKey, orgId, serviceId: "gpt3")
            .AddOpenAIChatCompletion("gpt-4", apiKey, orgId, serviceId: "gpt4")
            .Build();
        SimplePrompt();
        SematicFunctionPrompt();
        NativeFunctionPrompt();
        PluginPrompt();
        FunctionCallingPlanner();
        Console.ReadLine();
    }

    private static async void SimplePrompt()
    {
        string prompt = "Finish the following knock-knock joke. Knock, knock. Who's there? Dishes. Dishes who?";
        var joke = await kernel.InvokePromptAsync(prompt);
        Console.WriteLine(joke);
    }

    private static async void SematicFunctionPrompt()
    {
        string prompt = "Finish the following knock-knock joke. Knock, knock. Who's there? {{$input}}, {{$input}} who?";
        KernelFunction jokeFunction = kernel.CreateFunctionFromPrompt(prompt);
        var arguments = new KernelArguments() { ["input"] = "Boo" };
        var joke = await kernel.InvokeAsync(jokeFunction, arguments);
        Console.WriteLine(joke);
    }

    private  static async void NativeFunctionPrompt()
    {
        string prompt = "Finish the following knock-knock joke. Knock, knock. Who's there? {{$input}}, {{$input}} who?";
        KernelFunction jokeFunction = kernel.CreateFunctionFromPrompt(prompt);
        var showManagerPlugin = kernel.ImportPluginFromObject(new Plugins.ShowManager());
        var result = await kernel.InvokeAsync(showManagerPlugin["RandomTheme"]);
        Console.WriteLine("I will tell a joke about " + result);
        var arguments = new KernelArguments() { ["input"] = result };
        var joke = await kernel.InvokeAsync(jokeFunction, arguments);
        Console.WriteLine(joke);
    }

    private static async void PluginPrompt()
    {
        var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "jokes");
        var jokesPlugin = kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "jokes");
        var theme = new ShowManager().RandomTheme();
        var joke = await kernel.InvokeAsync(jokesPlugin["knock_knock_joke"], new KernelArguments() { ["input"] = theme.ToString() });
        Console.WriteLine(joke);
        var explanation = await kernel.InvokeAsync(jokesPlugin["explain_joke"], new KernelArguments() { ["input"] = joke });
        Console.WriteLine(explanation);
    }

    private static async void FunctionCallingPlanner()
    {
        var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "jokes");
        var goalFromUser = "Choose a random theme for a joke, generate a knock-knock joke about it and explain it";
        kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "jokes");
        kernel.ImportPluginFromObject(new Plugins.ShowManager());
#pragma warning disable SKEXP0060
        var plannerOptions = new HandlebarsPlannerOptions() { AllowLoops = false };
        var planner = new HandlebarsPlanner(plannerOptions);
        var plan = await planner.CreatePlanAsync(kernel, goalFromUser);
        var result = await plan.InvokeAsync(kernel);
#pragma warning restore SKEXP0060
        Console.WriteLine(result);
    }
}



