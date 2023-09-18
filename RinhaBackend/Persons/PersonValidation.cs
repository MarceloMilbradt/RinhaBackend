namespace RinhaBackend.Persons;


internal record CreatePersonResult(bool CanCreate, Guid Id)
{
    public static CreatePersonResult Fail() => new(false, Guid.Empty);
    public static CreatePersonResult Success(Guid id) => new(true, id);
}