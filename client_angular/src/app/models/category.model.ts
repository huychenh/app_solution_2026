export interface Category {  
  id?: string;
  name: string;
  description?: string;
  createdDate?: string | Date;
  updatedDate?: string | Date;
  createdBy?: string;
  updatedBy?: string;
  isActived: boolean;
  isDeleted: boolean;
}

export interface CreateCategory {
  name: string;
  description: string;
}