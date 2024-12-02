namespace Dal;
using DalApi;
using DO;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment in the XML file.
    /// </summary>
    /// <param name="item">The assignment to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when an assignment with the same ID already exists.</exception>
    public void Create(Assignment item)
    {
        var assignmentsXml = XMLTools.LoadListFromXMLElement(Config.Assignments_Xml);
        assignmentsXml.Add(AssignmentToXElement(item with { Id = Config.NextAssignmentId }));
        XMLTools.SaveListToXMLElement(assignmentsXml, Config.Assignments_Xml);
    }

    /// <summary>
    /// Deletes an assignment from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        var assignmentsXml = XMLTools.LoadListFromXMLElement(Config.Assignments_Xml);
        XElement? assignmentElem = assignmentsXml.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id) ?? throw new DalNotExistException($"Assignment with Id ={id} doesn't exist");
        assignmentElem.Remove();
        XMLTools.SaveListToXMLElement(assignmentsXml, Config.Assignments_Xml);
    }

    /// <summary>
    /// Deletes all assignments from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        var assignmentsXml = XMLTools.LoadListFromXMLElement(Config.Assignments_Xml);
        assignmentsXml.Elements().Remove();
        XMLTools.SaveListToXMLElement(assignmentsXml, Config.Assignments_Xml);
    }

    /// <summary>
    /// Reads an assignment from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    public Assignment? Read(int id)
    {
        XElement? assignmentElem = getAssignmentXElementFromId(id);

        return assignmentElem is null ? null : getAssignmentFromXElement(assignmentElem);
    }

    /// <summary>
    /// Reads an assignment from the XML file by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The assignment that matches the filter, or null if not found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.Assignments_Xml)
                                   .Elements()
                                   .Select(getAssignmentFromXElement);

        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments from the XML file, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all assignments.</param>
    /// <returns>An enumerable of assignments.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = from aElem in XMLTools.LoadListFromXMLElement(Config.Assignments_Xml).Elements()
                         select getAssignmentFromXElement(aElem);

        return filter is null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment in the XML file.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    /// <exception cref="DalNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        Delete(item.Id);

        var assignmentsXml = XMLTools.LoadListFromXMLElement(Config.Assignments_Xml);
        assignmentsXml.Add(AssignmentToXElement(item));

        XMLTools.SaveListToXMLElement(assignmentsXml, Config.Assignments_Xml);
    }

    /// <summary>
    /// Converts an XElement to an Assignment object.
    /// </summary>
    /// <param name="assignmentElem">The XElement representing the assignment.</param>
    /// <returns>The Assignment object.</returns>
    private Assignment getAssignmentFromXElement(XElement assignmentElem)
    {
        DateTime? endDate = null;
        if (!string.IsNullOrEmpty(assignmentElem.Element("EndTime")?.Value))
        {
            endDate = (DateTime?)assignmentElem.Element("EndTime");
        }

        AssignmentEndReason? endReason = null;
        if (!string.IsNullOrEmpty(assignmentElem.Element("AssignmentEndReason")?.Value))
        {
            endReason = (AssignmentEndReason?)Enum.Parse(typeof(AssignmentEndReason), assignmentElem.Element("AssignmentEndReason")?.Value!);
        }

        return new Assignment
        {
            Id = (int)assignmentElem.Element("Id")!,
            VolunteerId = (int)assignmentElem.Element("VolunteerId")!,
            CallId = (int)assignmentElem.Element("CallId")!,
            StartTime = (DateTime)assignmentElem.Element("StartTime")!,
            EndTime = endDate,
            EndReason = endReason
        };
    }

    /// <summary>
    /// Retrieves an assignment XElement by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment.</param>
    /// <returns>The XElement representing the assignment, or null if not found.</returns>
    private XElement? getAssignmentXElementFromId(int id)
    {
        return XMLTools.LoadListFromXMLElement(Config.Assignments_Xml)
                       .Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
    }

    /// <summary>
    /// Converts an Assignment object to an XElement.
    /// </summary>
    /// <param name="item">The Assignment object.</param>
    /// <returns>The XElement representing the assignment.</returns>
    private XElement AssignmentToXElement(Assignment item)
    {
        return new XElement("Assignment",
                        new XElement("Id", item.Id),
                        new XElement("VolunteerId", item.VolunteerId),
                        new XElement("CallId", item.CallId),
                        new XElement("StartTime", item.StartTime),
                        new XElement("EndTime", item.EndTime),
                        new XElement("AssignmentEndReason", item.EndReason)
                        );
    }
}
