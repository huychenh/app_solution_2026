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

  // 🟢 NEW: Added properties to manage server-side pagination state
  currentPage: number = 1;
  pageSize: number = 10; // Default page size configured at client
  totalCount: number = 0;
  totalPages: number = 0;

  // 🟢 NEW: Dropdown selection config options for items per page
  pageSizeOptions: number[] = [10, 20, 50];

  ngOnInit(): void {
    // Initial load: Fetch all categories with default configurations
    this.loadCategories();
  }

  // 🔄 MODIFIED: Updated signature to accept structural pagination parameters
  loadCategories(keyword: string = '', page: number = 1, size: number = this.pageSize): void {
    this.isLoading.set(true); // Turn on the loading spinner
    
    // 🟢 NEW: Sync navigation states prior to dispatching HTTP request
    this.currentPage = page;
    this.pageSize = size;

    // 🔄 MODIFIED: Pass page and pageSize down to the CategoryService API call
    this.categoryService.getCategories(keyword, this.currentPage, this.pageSize).subscribe({
      next: (response: any) => {        
        // 🔄 MODIFIED: Backend API now returns an object { items: [], totalCount: X } instead of a raw array
        this.categories.set(response.items); 
        
        // 🟢 NEW: Calculate metadata properties based on structural response
        this.totalCount = response.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);

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
    
    // 🔄 MODIFIED: Reset navigation flow back to Page 1 when initiating a new search context
    this.loadCategories(this.currentKeyword, 1);
  }

  // 🟢 NEW: Triggered when clicking pagination navigation control selectors
  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.loadCategories(this.currentKeyword, page);
  }

  // 🟢 NEW: Triggered when selecting a different size from the dropdown list picker
  // onPageSizeChange(event: Event): void {
  //   const selectElement = event.target as HTMLSelectElement;
  //   const newSize = Number(selectElement.value);
    
  //   // Reset layout flow back to Page 1 using the newly chosen size limit parameters
  //   this.loadCategories(this.currentKeyword, 1, newSize);
  // }

  onPageSizeChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    if (target) {
      // 🔄 Ép kiểu giá trị nhận được từ String sang Number
      this.pageSize = Number(target.value); 
      
      // Mỗi khi đổi page size, reset số trang về lại trang 1
      this.currentPage = 1; 
      
      // Gọi lại hàm load dữ liệu với kích thước mới
      this.loadCategories(this.currentKeyword, this.currentPage, this.pageSize);
    }
  }

  // DELETE: Trigger the database delete mechanism
  onDelete(id: string | undefined): void {
    if (!id) return;

    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          // 🔄 MODIFIED: Maintain the exact active page grid context upon removal
          this.loadCategories(this.currentKeyword, this.currentPage);
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