import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SystemTablesComponent } from './system-tables.component';
import { RouterTestingModule } from '@angular/router/testing';


describe('SystemTablesComponent', () => {
  let component: SystemTablesComponent;
  let fixture: ComponentFixture<SystemTablesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SystemTablesComponent, 
        RouterTestingModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SystemTablesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
