import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http'; // Importación de las dependencias necesarias
import { Injectable } from '@angular/core'; // Importación del decorador Injectable para indicar que es un servicio
import { of } from 'rxjs'; // Importación de la función of para trabajar con Observables
import { map } from 'rxjs/operators'; // Importación de la función map para transformar los datos de un Observable
import { environment } from 'src/environments/environment'; // Importación del archivo environment para obtener la URL del API
import { Member } from '../_models/member'; // Importación del modelo Member
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl; // URL del API
  members: Member[] = []; // Array que contendrá los miembros
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) { } // Se inyecta el HttpClient para hacer peticiones HTTP

  getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();

    if (page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }
    
    return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
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