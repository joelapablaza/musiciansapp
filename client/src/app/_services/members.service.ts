import { HttpClient, HttpHeaders } from '@angular/common/http'; // Importación de las dependencias necesarias
import { Injectable } from '@angular/core'; // Importación del decorador Injectable para indicar que es un servicio
import { of } from 'rxjs'; // Importación de la función of para trabajar con Observables
import { map } from 'rxjs/operators'; // Importación de la función map para transformar los datos de un Observable
import { environment } from 'src/environments/environment'; // Importación del archivo environment para obtener la URL del API
import { Member } from '../_models/member'; // Importación del modelo Member

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl; // URL del API
  members: Member[] = []; // Array que contendrá los miembros

  constructor(private http: HttpClient) { } // Se inyecta el HttpClient para hacer peticiones HTTP

  getMembers() {
    if (this.members.length > 0) return of(this.members); // Si ya se han obtenido los miembros, se devuelven
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe( // Si no, se hace una petición HTTP para obtenerlos
      map(members => {
        this.members = members; // Se asignan los miembros obtenidos al array members
        return members;
      })
    )
  }

  getMember(username: string) {
    const member = this.members.find(x => x.username === username); // Se busca el miembro en el array members
    if (member !== undefined) return of(member); // Si se encuentra, se devuelve
    return this.http.get<Member>(this.baseUrl + 'users/' + username); // Si no, se hace una petición HTTP para obtenerlo
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe( // Se actualizan los datos del miembro haciendo una petición HTTP PUT
      map(() => {
        const index = this.members.indexOf(member); // Se busca el índice del miembro en el array members
        this.members[index] = member; // Se actualiza el miembro en el array members
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}