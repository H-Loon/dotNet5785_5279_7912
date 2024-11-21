namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        if(Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Call with Id ={item.Id} already exists ");
        DataSource.Calls.Add(item with { Id = Config.NextCallId });
    }

    public void Delete(int id)
    {
        var call = Read(id);
        if (call is null)
            throw new DalNotExistException($"Call with Id ={id} doesn t exists");
        DataSource.Calls.Remove(call);
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Call> ReadAll(Func<Call,bool>?filter=null)
    { 
        return filter == null ?DataSource.Calls:DataSource.Calls.Where(filter);

    
    }

    public void Update(Call item)
    {
        var existingCall = Read(item.Id);
        if (existingCall is null)
            throw new DalNotExistException($"Call with Id ={item.Id} doesn t exists");
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Add(item);
        
    }
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
