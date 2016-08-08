using System.ServiceModel;
[ServiceContract]
public interface ISchedaHW
{
    [OperationContract]
    bool ImpostaLinea(byte numero, bool stato);
}