import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, switchMap } from 'rxjs';
import { Category } from '../models/category.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private readonly apiUrl = 'https://localhost:7210/api/categories';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAll(): Observable<Category[]> {
    return this.authService.getAccessToken().pipe(
      switchMap(token => {
        if (!token) {
          throw new Error('User not authenticated');
        }

        const headers = {
          Authorization: `Bearer ${token}`
        };

        return this.http.get<Category[]>(
          `${this.apiUrl}/list`,
          { headers }
        );
      })
    );
  }

  getById(id: number): Observable<Category> {
    return this.authService.getAccessToken().pipe(
      switchMap(token => {
        if (!token) {
          throw new Error('User not authenticated');
        }

        const headers = {
          Authorization: `Bearer ${token}`
        };

        return this.http.get<Category>(
          `${this.apiUrl}/getbyid/${id}`,
          { headers }
        );
      })
    );
  }

  create(category: Category): Observable<Category> {
    return this.authService.getAccessToken().pipe(
      switchMap(token => {
        if (!token) {
          throw new Error('User not authenticated');
        }

        const headers = {
          Authorization: `Bearer ${token}`
        };

        return this.http.post<Category>(
          `${this.apiUrl}/create`,
          category,
          { headers }
        );
      })
    );
  }

  update(id: number, category: Category): Observable<Category> {
    return this.authService.getAccessToken().pipe(
      switchMap(token => {
        if (!token) {
          throw new Error('User not authenticated');
        }

        const headers = {
          Authorization: `Bearer ${token}`
        };

        return this.http.put<Category>(
          `${this.apiUrl}/update/${id}`,
          category,
          { headers }
        );
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.authService.getAccessToken().pipe(
      switchMap(token => {
        if (!token) {
          throw new Error('User not authenticated');
        }

        const headers = {
          Authorization: `Bearer ${token}`
        };

        return this.http.delete<void>(
          `${this.apiUrl}/delete/${id}`,
          { headers }
        );
      })
    );
  }

}
