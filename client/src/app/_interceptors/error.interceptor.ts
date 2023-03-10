import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        // Manejo de error temporal, cuando este en produccion deberia de figurar el statusText correspondiente

        if (error) {
          switch (error.status) {
            // Casos error 400, primero maneja el error emitido desde el formulario de registro
            // Luego maneja el cualqueir otro error de tipo Bad Request
            case 400:
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key])
                  }
                }
                throw modalStateErrors.flat();
              } else if (typeof(error.error === 'object')) {
                this.toastr.error(error.statusText, error.status);
              } else {
                this.toastr.error(error.error, error.status);
              }
              break;

              // Muestra los errores desatados por status 401 Unauthorized
              // Manejo de error temporal para enviar mensajes por separado cuando el error
              // se desata en el form de login y para cuando se desata de otra parte
            case 401:
              if (error.status == 401) {
                if (error.error == "Invalid username or password") {
                  error.statusText = "Unauthorized";
                } else {
                  error.statusText = "Unauthorized";
                  this.toastr.error(error.statusText, error.status);
                }
              }
              
              break;

              // Se asegura que el satusText de un error 404 sea Not Found
              // Redirige a la pagina Not Found
            case 404:
              if (error.status == 404) {
                error.statusText = "Not Found"
              }
              this.router.navigateByUrl('/not-found');
              break;


            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}}
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    )
  }
}
