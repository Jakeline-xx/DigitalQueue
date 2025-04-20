namespace DigitalQueue.Domain.Models;

public struct PatientKey
{
    public PatientKey(string code) => Value = code;

    public string Value { get; set; }
}
