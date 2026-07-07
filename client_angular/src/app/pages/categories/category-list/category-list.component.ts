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
  // Inject dependencies using the inject() function
  private categoryService = inject(CategoryService);

  // Signal to store and manage the categories list displayed on the UI
  categories = signal<Category[]>([]);

  // Signal to handle the screen-level loading state for better UX
  isLoading = signal<boolean>(false);

  // Retain the current search keyword to preserve filters after a delete operation
  currentKeyword: string = '';

  ngOnInit(): void {
    // Initial load: Fetch all categories with an empty keyword when entering the page
    this.loadCategories();
  }

  // READ: Fetch categories from the API, supporting server-side filtering
  loadCategories(keyword: string = ''): void {
    this.isLoading.set(true); // Turn on the loading spinner

    this.categoryService.getCategories(keyword).subscribe({
      next: (data: Category[]) => {        
        this.categories.set(data); // Overwrite the signal with fresh server data
        this.isLoading.set(false); // Turn off the loading spinner upon success
      },
      error: (err) => {
        console.error('Error fetching categories data from server:', err);
        this.isLoading.set(false); // Ensure loading is turned off even if an error occurs
      }
    });
  }

  // SEARCH: Triggered when the user types a keyword and presses ENTER
  onSearch(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.currentKeyword = inputElement.value; // Synchronize the keyword state
    
    // Call the API to filter data directly on the database server
    this.loadCategories(this.currentKeyword);
  }

  // DELETE: Trigger the database delete mechanism
  onDelete(id: string | undefined): void {
    if (!id) return;

    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          // Re-fetch data using the current keyword to retain the user's active search filter
          this.loadCategories(this.currentKeyword);
        },
        error: (err) => {
          console.error('Error deleting category:', err);
        }
      });
    }
  }

  // MODAL DETAIL: Manage state and display mechanisms for the category detail modal
  selectedCategory = signal<any>(null);

  openDetailModal(item: any) {
    this.selectedCategory.set(item);
    
    const modalElement = document.getElementById('categoryDetailModal');
    if (modalElement) {
      const bootstrapModal = new (window as any).bootstrap.Modal(modalElement);
      bootstrapModal.show();
    }
  }
}