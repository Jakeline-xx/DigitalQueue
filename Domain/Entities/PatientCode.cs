namespace DigitalQueue.Domain.Entities;
public class PatientCode
{
    public string Code { get; set; }
    public bool IsPriority => Code.StartsWith("P");

    public PatientCode(string code)
    {
        Code = code;
    }
}