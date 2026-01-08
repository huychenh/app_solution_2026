import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Category } from '../../../models/category.model';

export type ModalMode = 'detail' | 'create' | 'edit' | 'delete';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.css']
})
export class CategoryFormComponent {

  @Input() visible = false;
  @Input() category: Category = {
    id: 0,
    name: '',
    description: '',
    createdDate: new Date(),
    updatedDate: new Date()
  };
  @Input() mode: ModalMode = 'detail';

  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<void>();
  @Output() update = new EventEmitter<Category>();
  @Output() create = new EventEmitter<Category>();
  @Output() deleteConfirm = new EventEmitter<number>();

  get isDetail() {
    return this.mode === 'detail';
  }

  get isEdit() {
    return this.mode === 'edit';
  }

  get isDelete() {
    return this.mode === 'delete';
  }

  get isCreate() {
    return this.mode === 'create';
  }

  onClose() {
    this.close.emit();
  }

  onEdit() {
    this.edit.emit();
  }

  onUpdate() {
    this.update.emit(this.category);
  }

  onCreate() {
    this.create.emit(this.category);
  }

  onDelete() {
    this.deleteConfirm.emit(this.category.id!);
  }
}
