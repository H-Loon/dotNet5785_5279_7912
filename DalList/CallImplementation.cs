namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        DataSource.Calls.Add(item with { Id = Config.NextCallId });
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
        return new List<Call>(DataSource.Calls);
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
