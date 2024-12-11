namespace Dal;
using DalApi;
using DO;
using System.Xml.Linq;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new call in the XML file.
    /// </summary>
    /// <param name="item">The call to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a call with the same ID already exists.</exception>
    public void Create(Call item)
    {
        var callsXml = XMLTools.LoadListFromXMLElement(Config.Calls_Xml);
        callsXml.Add(CallToXElement(item with { Id = Config.NextCallId }));
        XMLTools.SaveListToXMLElement(callsXml, Config.Calls_Xml);
    }

    /// <summary>
    /// Deletes a call from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the call to delete.</param>
    /// <exception cref="DalNotExistException">Thrown when a call with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        var callsXml = XMLTools.LoadListFromXMLElement(Config.Calls_Xml);
        XElement? callElem = callsXml.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);

        if (callElem is null)
            throw new DalDeletionImpossibleException($"Call with Id ={id} doesn't exist");

        callElem.Remove();
        XMLTools.SaveListToXMLElement(callsXml, Config.Calls_Xml);
    }

    /// <summary>
    /// Deletes all calls from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        var callsXml = XMLTools.LoadListFromXMLElement(Config.Calls_Xml);
        callsXml.Elements().Remove();
        XMLTools.SaveListToXMLElement(callsXml, Config.Calls_Xml);
    }

    /// <summary>
    /// Reads a call from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the call to read.</param>
    /// <returns>The call with the specified ID, or null if not found.</returns>
    public Call? Read(int id)
    {
        XElement? callElem = getCallXElementFromId(id);

        return callElem is null ? null : getCallFromXElement(callElem);
    }

    /// <summary>
    /// Reads a call from the XML file by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The call that matches the filter, or null if not found.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        var calls = XMLTools.LoadListFromXMLElement(Config.Calls_Xml)
                            .Elements()
                            .Select(getCallFromXElement);

        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all calls from the XML file, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all calls.</param>
    /// <returns>An enumerable of calls.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        var calls = from cElem in XMLTools.LoadListFromXMLElement(Config.Calls_Xml).Elements()
                    select getCallFromXElement(cElem);

        return filter is null ? calls : calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing call in the XML file.
    /// </summary>
    /// <param name="item">The call to update.</param>
    /// <exception cref="DalNotExistException">Thrown when a call with the specified ID does not exist.</exception>
    public void Update(Call item)
    {
        Delete(item.Id);

        var callsXml = XMLTools.LoadListFromXMLElement(Config.Calls_Xml);

        callsXml.Add(CallToXElement(item));

        XMLTools.SaveListToXMLElement(callsXml, Config.Calls_Xml);
    }

    /// <summary>
    /// Converts an XElement to a Call object.
    /// </summary>
    /// <param name="callElem">The XElement representing the call.</param>
    /// <returns>The Call object.</returns>
    private Call getCallFromXElement(XElement callElem)
    {
        return new Call
        {
            Id = (int)callElem.Element("Id")!,
            Type = (CallType)Enum.Parse(typeof(CallType), callElem.Element("Type")!.Value),
            Address = callElem.Element("Address")!.Value,
            Latitude = (double)callElem.Element("Latitude")!,
            Longitude = (double)callElem.Element("Longitude")!,
            StartTime = (DateTime)callElem.Element("StartTime")!,
            Description = callElem.Element("Description")?.Value,
            MaxTime = callElem.Element("MaxTime")?.Value.ToString() == "" ? null : (DateTime?)callElem.Element("MaxTime")!
        };
    }

    /// <summary>
    /// Retrieves a Call XElement by ID.
    /// </summary>
    /// <param name="id">The ID of the call.</param>
    /// <returns>The XElement representing the call, or null if not found.</returns>
    private XElement? getCallXElementFromId(int id)
    {
        return XMLTools.LoadListFromXMLElement(Config.Calls_Xml)
                       .Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
    }

    /// <summary>
    /// Converts a Call object to an XElement.
    /// </summary>
    /// <param name="item">The Call object.</param>
    /// <returns>The XElement representing the call.</returns>
    private XElement CallToXElement(Call item)
    {
        return new XElement("Call",
                    new XElement("Id", item.Id),
                    new XElement("Type", item.Type),
                    new XElement("Address", item.Address),
                    new XElement("Latitude", item.Latitude),
                    new XElement("Longitude", item.Longitude),
                    new XElement("StartTime", item.StartTime),
                    new XElement("Description", item.Description),
                    new XElement("MaxTime", item.MaxTime)
                    );
    }
}
