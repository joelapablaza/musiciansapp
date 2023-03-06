import { Component, HostListener, OnInit, ViewChild } from '@angular/core'; // Importación de las dependencias necesarias
import { NgForm } from '@angular/forms'; // Importación del NgForm para trabajar con formularios
import { ToastrService } from 'ngx-toastr'; // Importación del ToastrService para mostrar mensajes al usuario
import { take } from 'rxjs/operators'; // Importación de la función take para trabajar con Observables
import { Member } from 'src/app/_models/member'; // Importación del modelo Member
import { User } from 'src/app/_models/user'; // Importación del modelo User
import { AccountService } from 'src/app/_services/account.service'; // Importación del AccountService para obtener el usuario actual
import { MembersService } from 'src/app/_services/members.service'; // Importación del MembersService para trabajar con los miembros

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm; // Decorador que permite obtener el formulario editForm del template HTML
  member: Member; // Objeto tipo Member que se utilizará para actualizar los datos del usuario
  user: User; // Objeto tipo User que se utiliza para obtener el usuario actual
  @HostListener('window:beforeunload', ['$event']) // Decorador que permite ejecutar un evento antes de salir de la página
  unloadNotification($event: any) {
    if (this.editForm.dirty) { // Si el formulario está sucio (ha sido modificado), se muestra una notificación
      $event.returnValue = true; // Se establece que hay cambios sin guardar
    }
  }

  constructor(private accountService: AccountService, private memberService: MembersService,
    private toastr: ToastrService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user); // Se obtiene el usuario actual y se asigna a la variable user
  }

  ngOnInit(): void {
    this.loadMember(); // Se carga el miembro actual
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => { // Se obtiene el miembro actual
      this.member = member; // Se asigna el miembro actual a la variable member
    })
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => { // Se actualizan los datos del miembro
      this.toastr.success("Profile updates successfuly"); // Se muestra un mensaje de éxito
      this.editForm.reset(this.member); // Se reinicia el formulario con los datos actualizados
    })
  }

}