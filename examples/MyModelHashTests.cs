[Fact]
public void ProduceCorrectHashFromModel()
{
    IGuid id = new Guid();
    IString name = new String();

    IMyModel model = new MyModel(
        id,
        name
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(model);

    Assert.True(expected.SequenceEqual(actual));
}

[Fact]
public void ProduceCorrectHashFromValues()
{
    IGuid id = new Guid();
    IString name = new String();

    IMyModel model = new MyModel(
        id,
        name
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(
        id,
        name
    );

    Assert.True(expected.SequenceEqual(actual));
}

[Fact]
public void ProduceCorrectHashFromIdHash()
{
    IGuid id = new Guid();
    IString name = new String();

    IMyModel model = new MyModel(
        id,
        name
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(
        new Hash(id),
        name
    );

    Assert.True(expected.SequenceEqual(actual));
}

[Fact]
public void ProduceCorrectHashFromNameHash()
{
    IGuid id = new Guid();
    IString name = new String();

    IMyModel model = new MyModel(
        id,
        name
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(
        id,
        new Hash(name)
    );

    Assert.True(expected.SequenceEqual(actual));
}

[Fact]
public void ProduceCorrectHashFromHashes()
{
    IGuid id = new Guid();
    IString name = new String();

    IMyModel model = new MyModel(
        id,
        name
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(
        new Hash(id),
        new Hash(name)
    );

    Assert.True(expected.SequenceEqual(actual));
}

