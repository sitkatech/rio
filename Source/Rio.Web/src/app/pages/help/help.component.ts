import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.scss']
})
export class HelpComponent implements OnInit {

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
