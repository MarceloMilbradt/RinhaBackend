namespace RinhaBackend.Persons;


public sealed record CreatePersonResult(bool CanCreate, Guid Id)
{
    public static readonly CreatePersonResult Fail = new(false, Guid.Empty);
    public static CreatePersonResult Success(Guid id) => new(true, id);
}