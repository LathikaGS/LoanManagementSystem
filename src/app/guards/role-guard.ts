import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {

  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const allowedRoles = route.data['roles'] as string[] | undefined;
    const userRole = this.auth.getRole();

    if (!allowedRoles || allowedRoles.length === 0) {
      return true;
    }

    if (!userRole || !allowedRoles.includes(userRole)) {
      this.router.navigate(['/login']); 
      return false;
    }

    return true;
  }
}
