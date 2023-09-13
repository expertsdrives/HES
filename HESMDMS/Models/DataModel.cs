using Elmah.ContentSyndication;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace HESMDMS.Models
{
    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=DataModel")
        {
        }

        public virtual DbSet<tbl_Response> tbl_Response { get; set; }
        // Method to track changes in the "Items" table
        public void TrackChanges()
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.ObjectStateManagerChanged += ObjectStateManager_ObjectStateManagerChanged;
        }
        private void ObjectStateManager_ObjectStateManagerChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                if (e.Element is Item newItem)
                {
                    // Perform any action you want with the new item here
                    // For example, you could add the item to a collection, display it, etc.
                    // In a real application, you might want to use events to notify other parts of the application about the new entry.
                }
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
