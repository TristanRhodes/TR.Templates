using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DbApi.Model;


public class Item
{
    public Guid ItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
