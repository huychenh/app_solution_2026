import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CreateCategory } from '../../models/category.model';
import { GlobalConfiguration } from '../../core/config/global-configuration';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  // Base URL pointing to your backend API or local json-server
  
  private apiBaseUrl = GlobalConfiguration.apiBaseUrl;

  private http = inject(HttpClient);

  // READ ALL: Get all active/non-deleted categories
  getCategories(keyword: string = ''): Observable<Category[]> {

    //return this.http.get<Category[]>(`${this.apiBaseUrl}/categories/list`);

    let params = new HttpParams();
    
    if (keyword.trim()) {
      params = params.set('keyword', keyword.trim());
    }

    return this.http.get<Category[]>(`${this.apiBaseUrl}/categories/search`, { params });

  }

  // READ DETAIL: Get a single category record by its Id for editing
  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.apiBaseUrl}/categories/getbyid/${id}`);
  }

  // CREATE: Post a new category record to the database
  addCategory(category: CreateCategory): Observable<Category> {
    return this.http.post<Category>(`${this.apiBaseUrl}/categories/create`, category);
  }

  // UPDATE: Put/Update an existing category record by Id
  updateCategory(id: string, category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.apiBaseUrl}/categories/update/${id}`, category);
  }

  // DELETE: Delete a category record by Id 
  // (Note: If your backend supports Soft Delete, this will just update IsDeleted = true via a PATCH/PUT request)
  deleteCategory(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiBaseUrl}/categories/delete/${id}`);
  }
}