import { Component, Input, OnInit } from '@angular/core'; // Importación de las dependencias necesarias
import { FileUploader } from 'ng2-file-upload';// Importación del objeto FileUploader para subir archivos
import { take } from 'rxjs/operators'; // Importación de la función take para trabajar con Observables
import { Member } from 'src/app/_models/member'; // Importación del modelo Member
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user'; // Importación del modelo User
import { AccountService } from 'src/app/_services/account.service'; // Importación del servicio AccountService
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment'; // Importación del archivo environment para obtener la URL del API

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member; // Miembro que se está editando
  uploader: FileUploader; // Objeto FileUploader que se utilizará para subir la imagen
  hasBaseDropzoneOver = false; // Bandera que indica si se está arrastrando un archivo a la zona de subida
  baseUrl = environment.apiUrl; // URL del API
  user: User; // Usuario que está usando la aplicación

  constructor(private accountService: AccountService, private memberService: MembersService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user); // Se obtiene el usuario actual del servicio AccountService y se guarda en user
  }

  ngOnInit(): void {
    this.initializeUploader(); // Se inicializa el objeto FileUploader
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e; // Se actualiza la bandera hasBaseDropzoneOver al arrastrar un archivo a la zona de subida
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo', // URL del API para subir la imagen
      authToken: "Bearer " + this.user.token, // Token de autenticación del usuario para enviar junto con la petición
      isHTML5: true,
      allowedFileType: ['image'], // Tipos de archivo permitidos
      removeAfterUpload: true, // Se elimina el archivo localmente después de subirlo
      autoUpload: false, // No se sube el archivo automáticamente al seleccionarlo
      maxFileSize: 10 * 1024 * 1024 // Tamaño máximo del archivo en bytes (10 MB en este caso)
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false; // Se desactiva el envío de credenciales del usuario junto con la petición
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response); // Se convierte la respuesta del API (que debe ser un objeto JSON) en un objeto Photo
        this.member.photos.push(photo); // Se agrega la nueva foto al miembro actual
      }
    }
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
        if (p.isMain) p.isMain = false;
        if (p.id === photo.id) p.isMain = true;
      })
    })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id != photoId);
    })
  }
}
/*
Este código define un componente de Angular que muestra un editor de fotos para un miembro específico. El usuario puede seleccionar una imagen para subirla al servidor a través de la biblioteca ng2-file-upload. Algunos puntos importantes del código son:

Se utiliza el servicio AccountService para obtener el token de autenticación del usuario actual y enviarlo junto con la petición de subida de la imagen.
Se define un objeto FileUploader para configurar la subida de la imagen. En particular, se especifica la URL del API, el token de autenticación, los tipos de archivo permitidos, el tamaño máximo del archivo, entre otros.
Cuando se agrega un archivo al objeto FileUploader, se desactiva el envío de credenciales para evitar problemas de CORS.
Cuando se completa la subida de la imagen, se agrega la nueva foto al miembro actual. Para ello, se convierte la respuesta del API (que debe ser un objeto JSON) en un objeto Photo, que se agrega a la propiedad photos del miembro actual.*/