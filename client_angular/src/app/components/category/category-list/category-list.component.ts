import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../models/category.model';
import { CommonModule } from '@angular/common';
import { CategoryFormComponent } from '../category-form/category-form.component';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, CategoryFormComponent],
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css']
})
export class CategoryListComponent implements OnInit {

  categories: Category[] = [];
  isLoading = false;
  errorMessage = '';

  selectedCategory: Category = {
    id: 0,
    name: '',
    description: '',
    createdDate: new Date(),
    updatedDate: new Date()
  };

  showFormModal = false;
  modalMode: 'detail' | 'create' | 'edit' | 'delete' = 'detail';

  constructor(
    private categoryService: CategoryService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.showFormModal = false;
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.message || 'Cannot load category';
        this.isLoading = false;
      }
    });
  }

  onDelete(id?: number): void {
    if (!id) return;

    const confirmDelete = confirm('Are you sure you want to delete this?');
    if (!confirmDelete) return;

    this.categoryService.delete(id).subscribe(() => {
      this.loadCategories();
    });
  }

  openEditModal(id: number): void {

    if (!this.authService.isAdminRole()) {
      alert('You do not have permission.');
      return;
    }

    this.modalMode = 'edit';
    this.isLoading = true;

    this.categoryService.getById(id).subscribe({
      next: (res) => {
        this.selectedCategory = res;
        this.showFormModal = true;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Cannot load category detail';
        this.isLoading = false;
      }
    });
  }

  openDetailModal(id: number) {
    this.modalMode = 'detail';
    this.isLoading = true;

    this.categoryService.getById(id).subscribe({
      next: (res) => {
        this.selectedCategory = res;
        this.showFormModal = true;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Cannot load category detail';
        this.isLoading = false;
      }
    });
  }

  //openDeleteModal
  openDeleteModal(id: number) {

    if (!this.authService.isAdminRole()) {
      alert('You do not have permission.');
      return;
    }

    this.modalMode = 'delete';
    this.isLoading = true;

    this.categoryService.getById(id).subscribe({
      next: (res) => {
        this.selectedCategory = res;
        this.showFormModal = true;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Cannot load category detail';
        this.isLoading = false;
      }
    });
  }

  closeModal() {
    this.showFormModal = false;
  }

  openCreateModal() {

    if (!this.authService.isAdminRole()) {
      alert('You do not have permission.');
      return;
    }

    this.selectedCategory = {
      name: '',
      description: ''
    } as Category;

    this.modalMode = 'create';
    this.showFormModal = true;
  }

  createCategory(category: Category) {
    this.categoryService.create(category).subscribe(() => {
      this.closeModal();
      this.loadCategories();
    });
  }

  switchToEdit() {
    this.modalMode = 'edit';
  }

  updateCategory(category: Category) {
    this.categoryService
      .update(category.id!, category)
      .subscribe({
        next: () => {
          this.closeModal();
          this.loadCategories();
        },
        error: () => {
          this.errorMessage = 'Update category failed';
        }
      });
  }

  deleteCategory(id: number) {
    if (!id) return;

    this.categoryService.delete(id).subscribe({
      next: () => {
        this.closeModal();
        this.loadCategories();
      },
      error: () => {
        this.errorMessage = 'Delete category failed';
      }
    });
  }



}

