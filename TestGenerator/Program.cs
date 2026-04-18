namespace TestGenerator;

public static class Program
{
    static void Main()
    {
        string modelClass = "AxisRichRelationalModel";
        string className = modelClass + "Hash";
        string modelInterface = "I" + modelClass;

        var parameters = new[]
        {
            new Param("IGuid","id","new Guid()","new DeterminedHash(id)"),
            new Param("IGuid","chartId","new Guid()","new DeterminedHash(chartId)"),
            new Param("IString","legend","new RandomString()","new DeterminedHash((model as RelationalModel.Abstractions.IAxisRelationalModel).Legend)"),
            new Param("IString","legend2","new RandomString()","new DeterminedHash((model as RelationalModel.Abstractions.IAxisRelationalModel).Legend2)"),
            new Param("IString","legend3","new RandomString()","new DeterminedHash((model as RelationalModel.Abstractions.IAxisRelationalModel).Legend3)"),
        };

        Console.WriteLine(TestGenerator.Generate(className, modelInterface, modelClass, parameters));
    }
}
