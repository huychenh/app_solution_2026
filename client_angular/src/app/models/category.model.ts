export interface Category {
  // Core properties matching the Database columns
  id?: string;            // Maps to [Id]
  name: string;          // Maps to [Name]
  description?: string;   // Maps to [Description] (Optional if column allows NULL)
  
  // Audit logs for tracking system history
  createdDate?: string | Date; // Maps to [CreatedDate]
  updatedDate?: string | Date; // Maps to [UpdatedDate]
  createdBy?: string;          // Maps to [CreatedBy]
  updatedBy?: string;          // Maps to [UpdatedBy]

  // Status flags (Boolean properties)
  isActived: boolean;    // Maps to [IsActived] (Replaces the old 'status' property)
  isDeleted: boolean;    // Maps to [IsDeleted] (Used for Soft Delete mechanism)
}