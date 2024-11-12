namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call copy = item with { Id = Config.NextCallId };
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        if (Read(id) is Call call)
            DataSource.Calls.Remove(call);
        else
            throw new Exception($"Call with Id={id} does not exist");
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
       return DataSource.Calls.Find(c => c.Id == id);
    }

    public List<Call> ReadAll()
    {
        List<Call> NewCalls = new();
        foreach (var call in DataSource.Calls)
        {
            NewCalls.Add(call with { });
        }
        return NewCalls;
    }

    public void Update(Call item)
    {
        if (Read(item.Id) is Call call)
        {
            DataSource.Calls.Remove(call);
            DataSource.Calls.Add(item);
        }
        else
            throw new Exception($"Call with Id={item.Id} does not exist");
    }
}
