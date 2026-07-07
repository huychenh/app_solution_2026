import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CreateCategory } from '../../models/category.model';
import { GlobalConfiguration } from '../../core/config/global-configuration';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiBaseUrl = GlobalConfiguration.apiBaseUrl;
  private http = inject(HttpClient);

  // 🔄 MODIFIED: Updated signature to accept 'page' and 'pageSize', and changed return type to Observable<any>
  getCategories(keyword: string = '', page: number = 1, pageSize: number = 10): Observable<any> {
    
    // 🟢 NEW: Initialize HttpParams with pagination properties
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    // Append the search keyword filter if it is not empty
    if (keyword.trim()) {
      params = params.set('keyword', keyword.trim());
    }

    // 🔄 MODIFIED: URL pointed back to '/categories/search' to match your Backend Controller endpoint
    return this.http.get<any>(`${this.apiBaseUrl}/categories/search`, { params });
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
  deleteCategory(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiBaseUrl}/categories/delete/${id}`);
  }
}