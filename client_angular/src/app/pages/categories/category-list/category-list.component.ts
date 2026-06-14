import { Component, OnInit, signal, inject } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Category } from '../../../models/category.model';
import { CategoryService } from '../category.service';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})
export class CategoryListComponent implements OnInit {
  // 1. Inject dependencies using the inject() function instead of the traditional constructor
  private categoryService = inject(CategoryService);

  // 2. Wrap the raw array into a Signal, initialized with an empty array []
  categories = signal<Category[]>([]);

  ngOnInit(): void {
    this.loadCategories();
  }

  // READ: Fetch all categories from the API and update the Signal
  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (data: Category[]) => {        
        // 3. Update the Signal value using .set(). The UI will reactively re-render without ChangeDetectorRef
        this.categories.set(data);       
      },
      error: (err) => {
        console.error('Error fetching categories data from server:', err);
      }
    });
  }

  // DELETE: Trigger delete mechanism
  onDelete(id: string | undefined): void {
    if (!id) return;

    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          // Approach 1: Re-fetch the updated list from the server
          this.loadCategories();
          
          // Approach 2 (Optimistic UI): Update the local state directly without an extra API call
          // this.categories.update(current => current.filter(c => c.id !== id));
        },
        error: (err) => {
          console.error('Error deleting category:', err);
        }
      });
    }
  }
}