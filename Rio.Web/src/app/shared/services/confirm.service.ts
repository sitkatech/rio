import { Injectable } from '@angular/core';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmModalComponent } from '../components/confirm-modal/confirm-modal.component';

/**
 * Options passed when opening a confirmation modal
 */
 export interface ConfirmOptions {
  modalSize: string;
  /**
   * The title of the confirmation modal
   */
  title: string,

  /**
   * The message in the confirmation modal
   */
  message: string,

  buttonClassYes: string
  buttonDisabledYes?: boolean;
  buttonTextYes: string
  buttonTextNo: string
}

@Injectable({
    providedIn: 'root'
})
export class ConfirmState {
  /**
   * The last options passed ConfirmService.confirm()
   */
  options: ConfirmOptions;

  /**
   * The last opened confirmation modal
   */
  modal: NgbModalRef;

}

/**
 * A confirmation service, allowing to open a confirmation modal from anywhere and get back a promise.
 */
@Injectable({
    providedIn: 'root'
})

export class ConfirmService {
 
   constructor(private modalService: NgbModal, private state: ConfirmState) {}
 
   /**
    * Opens a confirmation modal
    * @param options the options for the modal (title and message)
    * @returns {Promise<boolean>} a promise that is fulfilled when the user chooses to confirm
    * or closes the modal
    */
   confirm(options: ConfirmOptions): Promise<boolean> {
     this.state.options = options;
     this.state.modal = this.modalService.open(ConfirmModalComponent, { ariaLabelledBy: options.title, backdrop: 'static', keyboard: false, size: options.modalSize ?? 'md' });
     return this.state.modal.result;
   }
 }
