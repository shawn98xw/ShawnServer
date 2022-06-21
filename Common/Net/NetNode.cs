namespace Common.Net;

public enum ENetNodeType
{
    Server = 0,
    Client = 1,
}
public class NetNode
{
    public ENetNodeType NodeType;
}