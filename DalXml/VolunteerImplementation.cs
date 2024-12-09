namespace Dal;
using DalApi;
using DO;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new volunteer in the XML file.
    /// </summary>
    /// <param name="item">The volunteer to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with Id ={item.Id} already exists ");

        var volunteersXml = XMLTools.LoadListFromXMLElement(Config.Volunteers_Xml);
        volunteersXml.Add(VolunteerToXElement(item));
        XMLTools.SaveListToXMLElement(volunteersXml, Config.Volunteers_Xml);
        
    }

    /// <summary>
    /// Deletes a volunteer from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    public void Delete(int id)  // AI helped me here
    {
        var volunteersXml = XMLTools.LoadListFromXMLElement(Config.Volunteers_Xml);
        XElement? volunteerElem = volunteersXml.Elements().FirstOrDefault(v => (int?)v.Element("Id") == id);

        if (volunteerElem is null)
            throw new DalDeletionImpossibleException($"Volunteer with Id ={id} doesn't exist");

        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersXml, Config.Volunteers_Xml);
    }

    /// <summary>
    /// Deletes all volunteers from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        var volunteersXml = XMLTools.LoadListFromXMLElement(Config.Volunteers_Xml);
        volunteersXml.Elements().Remove();
        XMLTools.SaveListToXMLElement(volunteersXml, Config.Volunteers_Xml);
    }

    /// <summary>
    /// Reads a volunteer from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to read.</param>
    /// <returns>The volunteer with the specified ID, or null if not found.</returns>
    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = getVolunteerXElementFromId(id);

        return volunteerElem is null ? null : getVolunteerFromXElement(volunteerElem);
    }

    /// <summary>
    /// Reads a volunteer from the XML file by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The volunteer that matches the filter, or null if not found.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        var volunteers = ReadAll();

        return volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all volunteers from the XML file, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all volunteers.</param>
    /// <returns>An enumerable of volunteers.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = from vElem in XMLTools.LoadListFromXMLElement(Config.Volunteers_Xml).Elements()
                         select getVolunteerFromXElement(vElem);

        return filter is null ? volunteers : volunteers.Where(filter);
    }

    /// <summary>
    /// Updates an existing volunteer in the XML file.
    /// </summary>
    /// <param name="item">The volunteer to update.</param>
    /// <exception cref="DalNotExistException">Thrown when the volunteer does not exist.</exception>
    public void Update(Volunteer item)
    {
        Delete(item.Id);
        var volunteersXml = XMLTools.LoadListFromXMLElement(Config.Volunteers_Xml);

        volunteersXml.Add(VolunteerToXElement(item));

        XMLTools.SaveListToXMLElement(volunteersXml, Config.Volunteers_Xml);
    }

    /// <summary>
    /// Converts an XElement to a Volunteer object.
    /// </summary>
    /// <param name="volunteerElem">The XElement representing the volunteer.</param>
    /// <returns>The Volunteer object.</returns>
    private Volunteer getVolunteerFromXElement(XElement volunteerElem)
    {
        return new Volunteer
        {
            Id = (int)volunteerElem.Element("Id")!,
            Name = volunteerElem.Element("Name")!.Value,
            Phone = volunteerElem.Element("Phone")!.Value,
            Email = volunteerElem.Element("Email")!.Value,
            Password = volunteerElem.Element("Password")?.Value ?? "",
            Address = volunteerElem.Element("Address")?.Value ?? "",
            Latitude = double.TryParse(volunteerElem.Element("Latitude")?.Value, System.Globalization.CultureInfo.InvariantCulture, out double lat) ? lat : (double?)null,
            Longitude = double.TryParse(volunteerElem.Element("Longitude")?.Value, System.Globalization.CultureInfo.InvariantCulture, out double lon) ? lon : (double?)null,
            Role = (RoleType)Enum.Parse(typeof(RoleType), volunteerElem.Element("Role")?.Value ?? throw new InvalidOperationException("Role element is missing")),
            IsActive = bool.Parse(volunteerElem.Element("IsActive")?.Value ?? throw new InvalidOperationException("IsActive element is missing")),
            MaxDistance = double.TryParse(volunteerElem.Element("MaxDistance")?.Value, out double maxDist) ? maxDist : (double?)null,
            DistanceType = (DistanceType)Enum.Parse(typeof(DistanceType), volunteerElem.Element("DistanceType")!.Value)
        };
    }

    /// <summary>
    /// Retrieves a volunteer XElement by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>The XElement representing the volunteer, or null if not found.</returns>
    private XElement? getVolunteerXElementFromId(int id)
    {
        return XMLTools.LoadListFromXMLElement(Config.Volunteers_Xml)
                       .Elements().FirstOrDefault(v => (int?)v.Element("Id") == id);
    }

    /// <summary>
    /// Converts a Volunteer object to an XElement.
    /// </summary>
    /// <param name="item">The Volunteer object.</param>
    /// <returns>The XElement representing the volunteer.</returns>
    private XElement VolunteerToXElement(Volunteer item)
    {
        return new XElement("Volunteer",
                        new XElement("Id", item.Id),
                        new XElement("Name", item.Name),
                        new XElement("Phone", item.Phone),
                        new XElement("Email", item.Email),
                        new XElement("Password", item.Password),
                        new XElement("Address", item.Address),
                        new XElement("Latitude", item.Latitude),
                        new XElement("Longitude", item.Longitude),
                        new XElement("IsActive", item.IsActive),
                        new XElement("Role", item.Role),
                        new XElement("MaxDistance", item.MaxDistance),
                        new XElement("DistanceType", item.DistanceType)
                        );
    }
}
