import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.scss']
})
export class HelpComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextType.Contact;

  constructor() { }

  ngOnInit() {
  }

  public getLeadOrganizationLongName() : string {
    return environment.leadOrganizationLongName;
  }

  public getContactInfoPhone() : string {
    return environment.contactInfoPhone;
  }

  public getContactInfoEmail() {
      return environment.contactInfoEmail;
  }

  public getContactInfoMailingAddress() {
      return environment.contactInfoMailingAddress;
  }

  public getContactInfoPhysicalAddress() {
      return environment.contactInfoPhysicalAddress;
  }

}
